using Pagos.Domain.Entity;
using Pagos.Domain.Enums;
using Pagos.Domain.interfaces.repositories;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.AccessControl;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json;

namespace Pagos.Infrastructure.Data;

public class PaymentProvider : IPaymentProvider
{
    private readonly string pagaFacil = "https://app-paga-chg-aviva.azurewebsites.net/swagger";
    private readonly string cazaPagos = "https://app-paga-chg-aviva.azurewebsites.net/swagger";
    private readonly string apiKey = "apikey-vcnmoisyhkif2s"; // API Key
    private static readonly HttpClient client = new HttpClient();
    private string providerName { get; set; }

    private List<Providers> ProviderList { get; set; }
    public PaymentProvider()
    {
        ProviderList = new List<Providers>
        {
            new Providers { Nombre = "PagaFacil", ApiUrl = "https://app-paga-chg-aviva.azurewebsites.net/Order", Tipo = "GET" },
            new Providers { Nombre = "PagaFacil", ApiUrl = "https://app-paga-chg-aviva.azurewebsites.net/Order", Tipo = "POST" },
            new Providers { Nombre = "PagaFacil", ApiUrl = "https://app-paga-chg-aviva.azurewebsites.net/Pay", Tipo = "PAY" },
            new Providers { Nombre = "PagaFacil", ApiUrl = "https://app-paga-chg-aviva.azurewebsites.net/Cancel", Tipo = "CANCEL" },
            new Providers { Nombre = "CazaPagos", ApiUrl = "https://app-caza-chg-aviva.azurewebsites.net/Order", Tipo = "GET" },
            new Providers { Nombre = "CazaPagos", ApiUrl = "https://app-caza-chg-aviva.azurewebsites.net/Order", Tipo = "POST" },
            new Providers { Nombre = "CazaPagos", ApiUrl = "https://app-caza-chg-aviva.azurewebsites.net/Pay", Tipo = "PAY" },
            new Providers { Nombre = "CazaPagos", ApiUrl = "https://app-caza-chg-aviva.azurewebsites.net/Cancel", Tipo = "CANCEL" }
            // Agrega más proveedores según sea necesario, aunque también se podrían traer directamente desde la base de datos
        };
    }

    public async Task<List<Order>?> GetOrders(string? provider)
    {
        List<Order>? result = new List<Order>();

        if (!string.IsNullOrEmpty(provider))
        {
            string url = ProviderList.Where(x => x.Nombre == provider && x.Tipo == "GET").FirstOrDefault().ApiUrl;

            var request = new HttpRequestMessage()
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(url)
            };

            request.Headers.Add("x-api-key", apiKey);

            try
            {
                var response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();

                // Lee el contenido de la respuesta
                string responseBody = await response.Content.ReadAsStringAsync();
                result = JsonSerializer.Deserialize<List<Order>>(responseBody);
            }
            catch (HttpRequestException ex)
            {
                string error = ex.Message;
            }
        }
        else
        {
            var providers = ProviderList.Where(x => x.Tipo == "GET").ToList();

            foreach (var provid in providers)
            {
                var request = new HttpRequestMessage()
                {
                    Method = HttpMethod.Get,
                    RequestUri = new Uri(provid.ApiUrl)
                };
                request.Headers.Add("x-api-key", apiKey);
                try
                {
                    var response = await client.SendAsync(request);
                    response.EnsureSuccessStatusCode();

                    // Lee el contenido de la respuesta
                    string responseBody = await response.Content.ReadAsStringAsync();
                    result.AddRange(JsonSerializer.Deserialize<List<Order>>(responseBody));
                }
                catch (HttpRequestException ex)
                {
                    string error = ex.Message;
                }
            }
        }

        return result;
    }

    public async Task<Order?> GetOrderById(string id)
    {
        //var result = new Order();
        Order? result = null;
        var providers = ProviderList.Where(x => x.Tipo == "GET").ToList();

        foreach (var provid in providers)
        {
            var request = new HttpRequestMessage()
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(provid.ApiUrl + $"/{id}")
            };
            request.Headers.Add("x-api-key", apiKey);
            try
            {
                var response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();

                // Lee el contenido de la respuesta
                string responseBody = await response.Content.ReadAsStringAsync();
                var res = JsonSerializer.Deserialize<Order>(responseBody);
                if (res != null)
                {
                    result = res;
                    providerName = provid.Nombre;
                    break;
                }
                
            }
            catch (HttpRequestException ex)
            {
                string error = ex.Message;
            }
        }

        return result;
    }

    public async Task<Order> CreateOrder(PaymentMethod method, List<Product> products)
    {
        var result = new Order();
        string provider = GetBetterProvider(method, products);

        string url = ProviderList.Where(x => x.Nombre == provider && x.Tipo == "POST").FirstOrDefault().ApiUrl;
        var data = new
        {
            method = method,
            products = products
        };
        var json = JsonSerializer.Serialize(data);
        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Post,
            Content = new StringContent(json, Encoding.UTF8, "application/json"),
            RequestUri = new Uri(url),
        };
        request.Headers.Add("x-api-key", apiKey);

        try
        {
            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            // Lee el contenido de la respuesta
            string responseBody = await response.Content.ReadAsStringAsync();
            result = JsonSerializer.Deserialize<Order?>(responseBody);
        }
        catch (HttpRequestException ex)
        {
            string error = ex.Message;
        }

        return result;
    }

    public async Task<string> CancelOrder(string id)
    {
        string result = string.Empty;
        Order? getOrder = await GetOrderById(id);

        var url = ProviderList.Where(x => x.Nombre == providerName && x.Tipo == "CANCEL").FirstOrDefault().ApiUrl;
        url += $"?id={id}";
        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Put,
            //Content = encodedContent,
            RequestUri = new Uri(url),
        };
        request.Headers.Add("x-api-key", apiKey);
        
        try
        {
            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            result = "Ok";
        }
        catch (HttpRequestException ex)
        {
            string error = ex.Message;
        }

        return result;
    }

    public async Task<string> PayOrder(string id)
    {
        string result = string.Empty;
        Order? getOrder = await GetOrderById(id);

        var url = ProviderList.Where(x => x.Nombre == providerName && x.Tipo == "PAY").FirstOrDefault().ApiUrl;
        url += $"?id={id}";
        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Put,
            //Content = encodedContent,
            RequestUri = new Uri(url),
        };
        request.Headers.Add("x-api-key", apiKey);

        try
        {
            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            result = "Ok";
        }
        catch (HttpRequestException ex)
        {
            string error = ex.Message;
        }

        return result;
    }

    private string GetBetterProvider(PaymentMethod method, List<Product> products)
    {
        var comisiones = GetComisionList();
        string provider = string.Empty;
        decimal total = 0;

        decimal monto = 0;
        foreach (var product in products)
        {
            monto += product.unitPrice;
        }

        foreach (var comision in comisiones)
        {
            var modalidadPago = comision.Modalidades.FirstOrDefault(m => m.Metodo == method);

            if (modalidadPago == null)
                continue;

            foreach (var regla in modalidadPago.ReglasComision)
            {
                if (monto > regla.MontoLimiteInferior && monto <= regla.MontoLimiteSuperior)
                {
                    regla.Monto = (regla.Porcentaje > 0) ? monto * regla.Porcentaje : 15;


                    if (provider == string.Empty && total == 0)
                    {
                        provider = comision.Nombre;
                        total = regla.Monto;
                    }
                    else
                    {
                        if (total > regla.Monto)
                        {
                            provider = comision.Nombre;
                            total = regla.Monto;
                        }
                    }
                }
            }
        }



        return provider;
    }

    private List<Comision> GetComisionList()
    {
        return new List<Comision>
        {
            new Comision
            {
                Nombre = "PagaFacil",
                Modalidades = new List<ModalidadPago>
                {
                    new ModalidadPago
                    {
                        Metodo = PaymentMethod.Cash,
                        ReglasComision = new List<ReglaComision>
                        {
                            new ReglaComision { Descripcion = "15 Pesos por Transacción", Porcentaje = 0, MontoLimiteInferior = 0, MontoLimiteSuperior = decimal.MaxValue, Monto = 0 }
                        }
                    },
                    new ModalidadPago
                    {
                        Metodo = PaymentMethod.CreditCard,
                        ReglasComision = new List<ReglaComision>
                        {
                            new ReglaComision { Descripcion = "1% del monto de Transacción", Porcentaje = 0.01m, MontoLimiteInferior = 0, MontoLimiteSuperior = decimal.MaxValue, Monto = 0 }
                        }
                    }
                }
            },
            new Comision
            {
                Nombre = "CazaPagos",
                Modalidades = new List<ModalidadPago>
                {
                    new ModalidadPago
                    {
                        Metodo = PaymentMethod.CreditCard,
                        ReglasComision = new List<ReglaComision>
                        {
                            new ReglaComision { Descripcion = "Monto entre 0 y 1500 → 2%", Porcentaje = 0.02m, MontoLimiteInferior = 0, MontoLimiteSuperior = 1500, Monto = 0 },
                            new ReglaComision { Descripcion = "Monto mayor a 1500 y hasta 5000 → 1.5%", Porcentaje = 0.015m, MontoLimiteInferior = 1500, MontoLimiteSuperior = 5000, Monto = 0 },
                            new ReglaComision { Descripcion = "Monto mayor a 5000 → 0.5%", Porcentaje = 0.005m, MontoLimiteInferior = 5000, MontoLimiteSuperior = decimal.MaxValue, Monto = 0 }
                        }
                    },
                    new ModalidadPago
                    {
                        Metodo = PaymentMethod.BankTransfer,
                        ReglasComision = new List<ReglaComision>
                        {
                            new ReglaComision { Descripcion = "Monto entre 0 y 500 → 5 Pesos", Porcentaje = 0, MontoLimiteInferior = 0, MontoLimiteSuperior = 500, Monto = 0 },
                            new ReglaComision { Descripcion = "Monto mayor a 500 y hasta 1000 → 2.5%", Porcentaje = 0.025m, MontoLimiteInferior = 500, MontoLimiteSuperior = 1000, Monto = 0 },
                            new ReglaComision { Descripcion = "Monto mayor a 1000 → 2.0%", Porcentaje = 0.02m, MontoLimiteInferior = 1000, MontoLimiteSuperior = decimal.MaxValue, Monto = 0 }
                        }
                    }
                }
            }
        };
    }

}
