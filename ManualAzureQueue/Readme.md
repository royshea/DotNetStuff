## Setup

Create an `App.SecretSettings.config` locate alongside the existing
`App.config` populated using the following XML:

```xml
<?xml version="1.0" encoding="utf-8" ?>

<appSettings>
  <add key="azureQueueUri" value="<URI to your azure queues>" />
  <add key="storageAccountKey" value="<Access queue for the storage account associated with the queues>" />
</appSettings>

```

To see more information in the demo application, it is recommend that
you manually create a queue within the storage account.
