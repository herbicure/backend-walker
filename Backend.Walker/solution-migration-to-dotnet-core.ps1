$paths = Get-ChildItem -include *.csproj -Recurse
foreach($pathobject in $paths) 
{
   cd $pathobject.directory.fullName
   try-convert
}