## Setup

Create an `App.SecretSettings.config` locate alongside the existing
`App.config` populated using the following XML:

```xml
<?xml version="1.0" encoding="utf-8" ?>

<appSettings>
    <add key="secretSetting"
         value="privateSetting" />
</appSettings>

```

Create an `App.SecretConnectionStrings.config` located alongside
the existing `App.config` populated using following XML:

```xml
<?xml version="1.0" encoding="utf-8" ?>

<connectionStrings>
    <add name="secretConnection"
         connectionString="server=secret;database=alsoSecret;"/>
</connectionStrings>
```

Note that both of these files are included in the project definition.
Also note that you must manually configure the build rule for this type
of configuration file to be always copied to output.  This has been done
for the two files listed above.

Use a `.gitignore` file to exclude these configurations from git.

## References

[Scott Hanselman blog post](https://www.hanselman.com/blog/BestPracticesForPrivateConfigDataAndConnectionStringsInConfigurationInASPNETAndAzure.aspx)
talking about keeping sensitive configurations out of source control.

[StackOverflow question](https://stackoverflow.com/questions/6940004/asp-net-web-config-configsource-vs-file-attributes
) addressing the difference between `file` and
`configSource` within an `App.config`.

