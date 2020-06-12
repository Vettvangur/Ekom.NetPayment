nuget restore
nuget pack .\src\Ekom.NetPayment\ -build -Symbols -SymbolPackageFormat snupkg
$pkg = gci *.nupkg 
nuget push $pkg -Source https://www.nuget.org/api/v2/package -NonInteractive
