using Pagos.Domain.Enums;

namespace Pagos.Domain.Entity
{
    public class Comision
    {
        public string Nombre { get; set; }
        public List<ModalidadPago> Modalidades { get; set; }
    }

    public class ModalidadPago
    {
        public PaymentMethod Metodo { get; set; }
        public List<ReglaComision> ReglasComision { get; set; }
    }

    public class ReglaComision
    {
        public string Descripcion { get; set; }
        public decimal Porcentaje { get; set; }
        public decimal MontoLimiteInferior { get; set; }
        public decimal MontoLimiteSuperior { get; set; }
        public decimal Monto {  get; set; }
    }
}
