$parameters = @{
    FilePath = 'c:\temp\emulatorcert.crt'
    CertStoreLocation = 'Cert:\CurrentUser\Root'
}

Import-Certificate @parameters