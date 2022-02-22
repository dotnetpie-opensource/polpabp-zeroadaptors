OutputDir:=nugets
Projects:=src/PolpAbp.ZeroAdaptors.Application/PolpAbp.ZeroAdaptors.Application.csproj \
src/PolpAbp.ZeroAdaptors.Application.Contracts/PolpAbp.ZeroAdaptors.Application.Contracts.csproj \
src/PolpAbp.ZeroAdaptors.Core/PolpAbp.ZeroAdaptors.Core.csproj \
src/PolpAbp.ZeroAdaptors.Core.Shared/PolpAbp.ZeroAdaptors.Core.Shared.csproj \
src/PolpAbp.ZeroAdaptors.Domain/PolpAbp.ZeroAdaptors.Domain.csproj

ProjectNames:=$(basename $(Projects))
Nugets:=$(addsuffix .nupkg,$(ProjectNames))

%.nupkg:
	@echo "Build $@"
ifndef Config
	@echo "Please provide the config with Config=debug or Config=release"
else
ifeq ($(Config),Release)
	@echo "Output release version"
	dotnet pack $(addsuffix .csproj,$(basename $@)) --output $(OutputDir) --configuration Release
else
	@echo "Output debug version"
	dotnet pack $(addsuffix .csproj,$(basename $@)) --include-symbols --output $(OutputDir)
endif
endif


build: $(Nugets)
	@echo "********************************"
	@echo "Build done"
	@echo "********************************"

pre-build:
	@echo "Create nugets dir"
	@mkdir nugets
	@echo "Done"

clean:
	@echo "Delete nugets"
	if [ -d nugets ]; then \
	  rm -rf nugets; \
        fi 
	@echo "Done"

push:
	@echo "Push to GitHub"
	git push

update-packages:
	@echo "Replace project references with packages"
ifndef Version
	@echo "Please provide a version with Version=1.xx"
else
	dotnet remove src/PolpAbp.ZeroAdaptors.Application/PolpAbp.ZeroAdaptors.Application.csproj reference ..\..\..\fwt\src\PolpAbp.Framework.Application\PolpAbp.Framework.Application.csproj
	dotnet remove src/PolpAbp.ZeroAdaptors.Application.Contracts/PolpAbp.ZeroAdaptors.Application.Contracts.csproj reference ..\..\..\fwt\src\PolpAbp.Framework.Application.Contracts\PolpAbp.Framework.Application.Contracts.csproj
	dotnet add src/PolpAbp.ZeroAdaptors.Application/PolpAbp.ZeroAdaptors.Application.csproj package PolpAbp.Framework.Application -v $(Version)
	dotnet add src/PolpAbp.ZeroAdaptors.Application.Contracts/PolpAbp.ZeroAdaptors.Application.Contracts.csproj package PolpAbp.Framework.Application.Contracts -v $(Version)
	dotnet remove tests/PolpAbp.ZeroAdaptors.TestBase/PolpAbp.ZeroAdaptors.TestBase.csproj reference ..\..\..\fwt\tests\PolpAbp.Framework.TestBase\PolpAbp.Framework.TestBase.csproj
	dotnet add tests/PolpAbp.ZeroAdaptors.TestBase/PolpAbp.ZeroAdaptors.TestBase.csproj package PolpAbp.Framework.TestBase -v $(Version)
	dotnet remove tests/PolpAbp.ZeroAdaptors.Application.Tests/PolpAbp.ZeroAdaptors.Application.Tests.csproj reference ..\..\..\fwt\tests\PolpAbp.Framework.Domain.Tests\PolpAbp.Framework.Domain.Tests.csproj
	dotnet add tests/PolpAbp.ZeroAdaptors.Application.Tests/PolpAbp.ZeroAdaptors.Application.Tests.csproj package PolpAbp.Framework.Domain.Tests -v $(Version)
endif

.PHONY: build pre-build clean push update-packages
