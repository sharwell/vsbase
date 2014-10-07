param($installPath, $toolsPath, $package, $project)

foreach ($reference in $project.Object.References)
{
	switch -regex ($reference.Name.ToLowerInvariant())
	{
	"^tvl\.visualstudio\.text\.editornavigation\.interfaces$"
		{
			$reference.CopyLocal = $false;
		}
	default
		{
			# ignore
		}
	}
}
