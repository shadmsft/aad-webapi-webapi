#Sample on how to creeate an AAD App registration for a SPA Application, in this case for our Postman Client
# Install-Module AzureAD
# Install-Module Azure.AzAccount

$tenatId = "<your AAD Tenant ID"
$userName = "<your username@domain.com"
$userPassword = "<your password>"

# AAD PowerShell Module Login
$creds = Get-Credential
Login-AzAccount -Credential $creds
Connect-AzureAD -TenantId $tenatId

#Mixing Azure AD Module and Azure CLI(to get access token)
#Login with Azure CLI
az login -u $userName -p $userPassword

#Create Client App Registration - in this case it's for a Postman Client
$appPostmanName = "apiPostMan2"
$appURI = "https://oauth.pstmn.io/v1/callback"
$appReplyURLs = @($appURI)
$myApp = New-AzureADApplication -DisplayName $appPostmanName -ReplyUrls $appReplyURLs -Oauth2AllowImplicitFlow $true   

$redirectUris = @()
$webAppUrl = "https://oauth.pstmn.io/v1/callback"
if ($redirectUris -notcontains "$webAppUrl") {
    $redirectUris += "$webAppUrl"   
    Write-Host "Adding $webAppUrl to redirect URIs";
}

$accesstoken =(az account get-access-token --resource="https://graph.microsoft.com/" --query accessToken --output tsv)

$header = @{
    'Content-Type' = 'application/json'
    'Authorization' = 'Bearer ' + $accesstoken
}
$body = @{
    'spa' = @{
        'redirectUris' = $redirectUris
    }
} | ConvertTo-Json

$graphURI = "https://graph.microsoft.com/v1.0/applications/$myApp.ObjectId"
Invoke-RestMethod -Method Patch -Uri $graphURI -Headers $header -Body $body

 