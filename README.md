# plugin.asm.dependency-manager
This repo hosts the dependency manager for Advanced Scene Manager.

> The reason this is hosted here, instead of inside asset itself, is because it is bundled as a dll. This ensures that the dependency manager will still load and be functional even when user has compilation errors.
Bundling source in asset store package will prompt unity to compile source as well in addition to dll, which obviously causes conflicts. Thus, this repo.
