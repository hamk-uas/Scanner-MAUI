# Scanner-MAUI

## Description 📝

This project aimed to build a Windows app the reads data form a portable network scanner, which records among other things LoRa network signal strength and location.  

---

## The app has the following features:
1. Oncoming real-time data reading and visualization directly from the scanner through serial port connection.
   
2. Users have the ability to choose the baud rate and "COM" port number.
   
3. Show the network location on the map.
   
4. Ability to read historical data from its SD card and apply filters based on the network name, and clear the SD card.
   
5. When reading the SD card the application generates a CSV file that includes the data.

---

## Configuration:
When cloning the repository to your device make sure to create a "Secrets" file that includes the necessary API keys. Follow the next instructions:
   
   1. Right click on the project name.
   
   2. Select "Manage User Secrests".
   
   3. Rename the file to your liking. Make sure it is a json file. Example ``yourfileName.josn``. The file content should look like:
   
        ```
        {
            "Settings": {
                "API-key": "your api key",
                "EsriAPI": "your api key"
            }
        }
        ```
   4. To get the API key from maanmittauslaitos, an account with them is necessary [maanmittauslaitos](https://omatili.maanmittauslaitos.fi/user/new/avoimet-rajapintapalvelut).
   
   5. For the second API key a developer account with Esri is necessary. [Esri](https://www.arcgis.com/sharing/rest/oauth2/authorize?client_id=arcgisdevelopers&response_type=code&expiration=20160&redirect_uri=https%3A%2F%2Fdevelopers.arcgis.com%2Fpost-sign-in%2F&state=%7B%22id%22%3A%22sxkGmqfxywELCWGLIQGDOf2bIAiZW72cU6ndw_A2qa4%22%2C%22originalUrl%22%3A%22https%3A%2F%2Fdevelopers.arcgis.com%2F%22%7D&locale=&style=&code_challenge_method=S256&code_challenge=gTGFZJHd3dveNINznCZTOeqGLBIYb_nZvucd-Hk89VM&showSignupOption=true&signuptype=developers).

   6. Make sure the json file name mathches the file name in the ``Directory.Build.targets`` file.

For debugging comment these lines in the ``Scanner-MAUI.csproj`` file:

    ```
    <TargetFrameworks>net7.0-android;net7.0-ios;net7.0-maccatalyst</TargetFrameworks>
    <SupportedOSPlatformVersion Condition="$([MSBuild]::GetTargetPlatformIdentifier('$(TargetFramework)')) == 'ios'">11.0</SupportedOSPlatformVersion>
    <SupportedOSPlatformVersion Condition="$([MSBuild]::GetTargetPlatformIdentifier('$(TargetFramework)')) == 'maccatalyst'">13.1</SupportedOSPlatformVersion>
    <SupportedOSPlatformVersion Condition="$([MSBuild]::GetTargetPlatformIdentifier('$(TargetFramework)')) == 'android'">21.0</SupportedOSPlatformVersion>
    ```

        
            
        
And for publishing uncomment them.
   
---
