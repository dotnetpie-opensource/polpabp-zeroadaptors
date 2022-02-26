# polpabp-zeroadaptors

# Develop

Switch to main branch.

# Release

1. Switch to release branch
2. Merge the main branch
> git merge main (cygwin)
3. Update the project references to package references 
> make update-packages Version=x... (powershell)
4. Bump the repo version
> bump version (cygwin)
5. Update project versions
> gulp (cygwin)
6. Clean up the nugets folder
> make clean (cgywin)
7. Build
> make build Config=Release (powershell)

# Deploy
1. Push to the nuget center
> make deploy -f Makefile.deploy NugetSource=xx (powercell)
