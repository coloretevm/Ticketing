# Sistema de Ticketing Tecnidro SRL

Sistema interno de ticketing para Tecnidro SRL, pensado para ejecutarse en un servidor local con Windows Server 2019 y administrarse desde navegador.

## Objetivo

Crear una aplicacion web interna que convierta correos entrantes de Outlook/Microsoft 365 en tickets, permita gestionarlos por usuarios internos y envie una respuesta al cliente cuando el ticket se cierre.

## Stack elegido

- .NET 8
- ASP.NET Core
- Blazor Server
- MudBlazor
- SQLite
- Entity Framework Core
- ASP.NET Core Identity
- Microsoft Graph SDK

## Funciones principales

- Ingesta automatica desde un buzon Outlook/Microsoft 365.
- Creacion automatica de tickets desde emails entrantes.
- Roles de usuario: `superadmin`, `admin`, `usuario`.
- Gestion de usuarios: crear, editar, desactivar y cambiar rol.
- Asignacion de tickets a usuarios internos.
- Cierre obligatorio con texto de resolucion.
- Envio de email al cliente al cerrar el ticket.
- Historial de comentarios, resoluciones y correos.
- Notificaciones internas por email para tickets importantes.
- Base de datos embebida con SQLite.
- Publicacion final como aplicacion local instalable como Windows Service.

## Modelo de datos inicial

### Users

- Id
- Email
- NombreCompleto
- Rol
- Activo

### Tickets

- Id
- Numero
- Asunto
- CuerpoOriginal
- EmailCliente
- NombreCliente
- Estado
- Prioridad
- AsignadoAUserId
- CreadoEn
- CerradoEn
- ResolucionTexto
- MessageIdOutlook

### TicketComentarios

- Id
- TicketId
- UserId
- Texto
- CreadoEn

### TicketAdjuntos

- Id
- TicketId
- NombreArchivo
- Ruta
- TamanoBytes

### EmailLog

- Id
- TicketId
- Tipo
- AsuntoEnviado
- Destinatario
- CreadoEn
- Estado

## Desarrollo local

Requisitos:

- Windows 11 para desarrollo.
- .NET 8 SDK instalado.

Cuando el SDK este instalado, la aplicacion se podra crear, compilar y ejecutar localmente con comandos `dotnet`.

## Despliegue previsto

La misma aplicacion desarrollada en Windows 11 se publicara para Windows Server 2019 como ejecutable self-contained y se instalara como Windows Service.

## Decisiones pendientes

- Buzon de Outlook/Microsoft 365 que se usara para soporte.
- Criterio exacto para marcar un ticket como importante.
- Si la respuesta al cliente se enviara dentro del mismo hilo del correo original.
- Politica de descarga y almacenamiento de adjuntos.
