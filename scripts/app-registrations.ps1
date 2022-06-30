#Install-Module AzureAD
#Install-Module Azure.AzAccount
#$creds = Get-Credential
#Login-AzAccount -Credential $creds
#Mixing Azure AD Module and Azure CLI(to get access token)

$tenatId
$userName 
$userPassword 

Connect-AzureAD -TenantId "<your tenant id>"
#Login with Azure CLI
az login -u <username@domain.com> -p <password>

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
#Invoke-RestMethod -Method Patch -Uri "https://graph.microsoft.com/v1.0/applications/$objectId" -Headers $header -Body $body
 