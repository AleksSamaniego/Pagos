# Order Management API

Esta API permite gestionar órdenes mediante diversas acciones como crear, consultar, pagar y cancelar órdenes.

## Funcionalidades

1. **Crear Orden**  
   Permite registrar una nueva orden en el sistema.

2. **Traer Órdenes**  
   Recupera la lista de todas las órdenes existentes.

3. **Traer Orden por ID**  
   Obtiene los detalles de una orden específica a través de su ID.

4. **Pagar Orden**  
   Actualiza el estado de una orden para indicar que ha sido pagada.

5. **Cancelar Orden**  
   Permite cancelar una orden pendiente o activa.

## Tecnologías

- **Lenguaje**: C#
- **Framework**: .NET Core, AutoMapper 
- **Patrón**: REST API, CQRS, DTO y IRepositories
