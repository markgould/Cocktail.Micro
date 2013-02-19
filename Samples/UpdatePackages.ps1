﻿foreach ($solution in dir -Path .\ -Filter "*.sln" -Recurse)
{
    $solutionDir = [IO.Path]::GetDirectoryName($solution.FullName)
    $packages = [IO.Path]::Combine($solutionDir, "packages")

    # Restore packages
    foreach ($pkg in dir -Path $solutionDir -Filter "packages.config" -Recurse)
    {
        nuget install $pkg.FullName -OutputDirectory $packages
    }

    # Update packages
    nuget update $solution.FullName -Id IdeaBlade.Cocktail -Id IdeaBlade.Cocktail.Utils -Id IdeaBlade.DevForce.Core -Id IdeaBlade.DevForce.Aop -Id IdeaBlade.DevForce.Server

    # Delete packages folder. It will be restored on next build
    if (Test-Path -Path $packages)
    {
        del $packages -Recurse
    }
}