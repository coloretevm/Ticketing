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

Comandos principales:

```powershell
dotnet restore
dotnet build
dotnet tool restore
dotnet tool run dotnet-ef database update --project src\Tecnidro.Ticketing\Tecnidro.Ticketing.csproj --startup-project src\Tecnidro.Ticketing\Tecnidro.Ticketing.csproj
dotnet run --project src\Tecnidro.Ticketing\Tecnidro.Ticketing.csproj --launch-profile http
```

Durante el desarrollo local la app queda disponible en:

```text
http://localhost:5274
```

El dashboard y la pantalla de tickets leen los tickets desde SQLite. La sincronizacion con Outlook se ejecuta desde el boton `Sincronizar Outlook`.

## Configuracion Outlook / Microsoft Graph

Para importar emails reales desde Outlook/Microsoft 365 hace falta una App Registration en Entra ID con permisos de aplicacion para leer el buzon de soporte.

Valores necesarios para desarrollo local:

- `TenantId`
- `ClientId`
- `ClientSecret`
- `MailboxAddress`, por ejemplo `soporte@tecnidro.it`

Configurar secretos locales:

```powershell
dotnet user-secrets set "GraphMailbox:TenantId" "TU_TENANT_ID" --project src\Tecnidro.Ticketing\Tecnidro.Ticketing.csproj
dotnet user-secrets set "GraphMailbox:ClientId" "TU_CLIENT_ID" --project src\Tecnidro.Ticketing\Tecnidro.Ticketing.csproj
dotnet user-secrets set "GraphMailbox:ClientSecret" "TU_CLIENT_SECRET" --project src\Tecnidro.Ticketing\Tecnidro.Ticketing.csproj
dotnet user-secrets set "GraphMailbox:MailboxAddress" "soporte@tecnidro.it" --project src\Tecnidro.Ticketing\Tecnidro.Ticketing.csproj
```

Permiso minimo previsto en Microsoft Graph:

- `Mail.ReadWrite` de tipo `Application`
- Admin consent concedido en el tenant

La pantalla `Tickets` tiene el boton `Sincronizar Outlook`. Lee mensajes no leidos del Inbox, crea tickets nuevos en SQLite y marca esos mensajes como leidos si `GraphMailbox:MarkMessagesAsRead` esta activo.

## Despliegue previsto

La misma aplicacion desarrollada en Windows 11 se publicara para Windows Server 2019 como ejecutable self-contained y se instalara como Windows Service.

## Decisiones pendientes

- Buzon de Outlook/Microsoft 365 que se usara para soporte.
- Criterio exacto para marcar un ticket como importante.
- Si la respuesta al cliente se enviara dentro del mismo hilo del correo original.
- Politica de descarga y almacenamiento de adjuntos.
