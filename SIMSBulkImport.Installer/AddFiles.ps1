$filehead = "<?xml version=""1.0"" encoding=""UTF-8""?>`n<Wix xmlns=""http://schemas.microsoft.com/wix/2006/wi"">`n   <Fragment>`n        <ComponentGroup Id=""SIMSBulkImportComponents"" Directory=""INSTALLDIR"">`n"

$filecontent = ""

$fileEntries = Get-ChildItem "..\build" -Exclude *vshost.exe*, *.xml, *.pdb
foreach($fileName in $fileEntries) 
{ 
	$cleanFilename = $fileName.Name -replace " ", ""
    $filecontent = $filecontent + "`n           <Component>`n               <File Id=""$cleanFilename"" Source=""..build\$fileName.Name"" />`n           </Component>"
}      

$filefoot = "`n`n		</ComponentGroup>`n	</Fragment>`n</Wix>"

$file = $filehead + $filecontent + $filefoot | Out-File SimsBulkImport.wxs -Encoding "UTF8"