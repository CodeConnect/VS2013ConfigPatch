# VS2013ConfigPatch
Patch for the Visual Studio 2013 configuration to use MSBuild version 2015

This patch addresses [issue #10: Alive cannot (currently) run WPF projects on VS2013](https://github.com/CodeConnect/AliveFeedback/issues/10)

Changes the following parts of `C:\Program Files (x86)\Microsoft Visual Studio 12.0\Common7\IDE\devenv.exe.config`
```xml
      <dependentAssembly>
        <assemblyIdentity name="Microsoft.Build.Framework" publicKeyToken="b03f5f7f11d50a3a" culture="neutral" />
        <bindingRedirect oldVersion="2.0.0.0 - 4.0.0.0" newVersion="12.0.0.0" />
      </dependentAssembly>
      <dependentAssembly>
        <assemblyIdentity name="Microsoft.Build" publicKeyToken="b03f5f7f11d50a3a" culture="neutral" />
        <bindingRedirect oldVersion="2.0.0.0 - 4.0.0.0" newVersion="12.0.0.0" />
      </dependentAssembly>
      <dependentAssembly>
        <assemblyIdentity name="Microsoft.Build.Engine" publicKeyToken="b03f5f7f11d50a3a" culture="neutral" />
        <bindingRedirect oldVersion="2.0.0.0 - 4.0.0.0" newVersion="12.0.0.0" />
      </dependentAssembly>
```
into
```xml
      <dependentAssembly>
        <assemblyIdentity name="Microsoft.Build.Framework" publicKeyToken="b03f5f7f11d50a3a" culture="neutral" />
        <bindingRedirect oldVersion="2.0.0.0 - 12.0.0.0" newVersion="14.0.0.0" />
      </dependentAssembly>
      <dependentAssembly>
        <assemblyIdentity name="Microsoft.Build" publicKeyToken="b03f5f7f11d50a3a" culture="neutral" />
        <bindingRedirect oldVersion="2.0.0.0 - 12.0.0.0" newVersion="14.0.0.0" />
      </dependentAssembly>
      <dependentAssembly>
        <assemblyIdentity name="Microsoft.Build.Engine" publicKeyToken="b03f5f7f11d50a3a" culture="neutral" />
        <bindingRedirect oldVersion="2.0.0.0 - 12.0.0.0" newVersion="14.0.0.0" />
      </dependentAssembly>
```

The script also creates a backup of `C:\Program Files (x86)\Microsoft Visual Studio 12.0\Common7\IDE\devenv.exe.config`
into `C:\Program Files (x86)\Microsoft Visual Studio 12.0\Common7\IDE\devenv.exe.config.backup`

When ran with argument `-u`, for example `VS2013ConfigPatch.exe -u`,
it creates a backup of the config file into ``C:\Program Files (x86)\Microsoft Visual Studio 12.0\Common7\IDE\devenv.exe.config.undoBackup` and un-does the changes to the config file.

Should you ever need to run this program more than once, you need to secure and manually remove the backup files. We err on the safe side and don't overwrite existing backups.
