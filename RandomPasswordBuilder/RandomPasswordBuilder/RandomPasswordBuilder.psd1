@{
    RootModule              =   'DrLSDee.Text.RandomPasswordBuilder.dll'
    Author                  =   'drlsdee'
    CompanyName             =   'drlsdee'
    ModuleVersion           =   '1.0.0.0'
    GUID                    =   '31b7ddf5-b5e2-4a1a-9279-0e132f6f2bea'
    Copyright               =   '2024 (c) drlsdee'
    Description             =   'This is just another .NET library generating random strings that are guaranteed to include all specified character categories.'
    PowerShellVersion       =   '5.1'
    CompatiblePSEditions    =   'Desktop'
    #   List of required assemblies and modules
    #RequiredAssemblies     =   @()
    #RequiredModules        =   @()
    FunctionsToExport       =   '*'
    AliasesToExport         =   '*'
    VariablesToExport       =   '*'
    #FormatsToProcess       =   'DrLSDee.Text.RandomPasswordBuilder.dll-format.ps1xml'
    PrivateData             =   @{
        PSData              =   @{
            Tags            =   @('PSModule','Password','Random','AD','Windows')
            #LicenseUri     =   ''
            ProjectUri      =   'https://github.com/drlsdee/RandomPasswordBuilder'
            #IconUri        =   ''
            ReleaseNotes    =   @'
The library currently uses .NET Framework 4.8, not .NET or .NET Core.

This is because the core product is being developed for use on general Windows/Windows Server installations where newer runtimes may not be installed.

But feel free to fork this project and adapt it for runtime you prefer.
'@
        }
    }
    #HelpInfoURI            =   ''
}