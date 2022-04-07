# CSCG3DBAGPipeline

  C# applicatie voor het downloaden van 3D BAG CityJSON bestanden, deze verwerken (filteren, verplaatsen naar maaiveld, etc.) en omzetten naar binaire glTF, comprimeren en uiteindelijk omzetten in B3DM.
  
  **Functionaliteit**
  - Downloaden van CityJSON bestanden;
  - Upgraden, filteren, converteren, comprimeren, etc.

  **Dependencies**
  - Aangepaste versie van [CityJSON/io](https://github.com/cityjson/cjio), ontwikkeld door tudelft3d;
  - [glTF-Transform/CLI](https://github.com/donmccurdy/glTF-Transform), ontwikkeld door Don McCurdy en bijdragers;
  - [b3dm-tile-cs](https://github.com/bertt/b3dm-tile-cs), ontwikkeld door Bert Temme;
  - [CliWrap](https://github.com/Tyrrrz/CliWrap), ontwikkeld door Oleksii Holub en bijgragers;
  - [Command Line](https://github.com/commandlineparser/commandline), door Eric Newton en bijdragers.

# Gebruik

## Download tegel 1 tot en met 25 van 3D BAG en zet deze om naar B3DM
```
    $ CSCG3DBAGPipeline.exe b3dm -s 1 -e 25 --filterscript "D:\workingdir\files\cjio-upgrade-filter.bat" --glbscript "D:\workingdir\files\cjio-glb.bat" --dracoscript "D:\workingdir\files\gltf-transform-draco-edgebreaker.bat" -u "https://data.3dbag.nl/cityjson/v210908_fd2cee53/3dbag_v210908_fd2cee53_{0}"
```

## Zet alle CityJSON tegels welke voldoen aan REGEX in directory /files/maaiveld om naar een tileset.json
```
    $ CSCG3DBAGPipeline.exe tileset -c "D:\workingdir\files\maaiveld"
```

## Voeg nieuwe CityJSON tegels (in /files/morecj) toe aan tileset.json
```
    $ CSCG3DBAGPipeline.exe tileset -c "D:\workingdir\files\morecj" -n "appenedtileset.json" -i "D:\workingdir\tileset.json"
```

## Opmerkingen

Standaard regex selecteer bestanden in het formaat `moved_{number}.json`. Eigen regex kan worden gebruikt: `--cityjsonregex`

Gebruik onderstaande commando om alle opties te zien:
```
    $ CSCG3DBAGPipeline.exe
    $ CSCG3DBAGPipeline.exe b3dm --help
    $ CSCG3DBAGPipeline.exe tileset --help
```
