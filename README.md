# polpabp-zeroadaptors

# Develop

Switch to main branch.

# Release

1. Switch to release branch
2. Run
> git merge main
3. Run 
> dnt switch-to-packages
4. Run (cygwin)
> bump version
5. Run (cygwin)
> gulp (or npm run bump)
6. Run (cygwin)
> make clean
7. Run (powershell)
> make build Config=Release 

# Deploy
1. Run 
> make deploy -f Makefile.deploy NugetSource=xx
