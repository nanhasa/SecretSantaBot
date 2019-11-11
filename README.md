# Secret Santa Bot

Secret Santa Bot is an command line tool to send participants an email telling who they are supposed to get a gift for. It is built with .NET Core 3.0 F#.

## How to configure

To run the app, it needs

- csv file containing participants
- app.config xml file

### Participants

The csv file should contain two columns name and email. The repository has an example version, that the app uses for schema validation [example-participants.csv](./src/example-participants.csv). Create a new file with the same schema and participants of your choosing. It doesn't matter where you save it as the app has mandatory command line argument where you need to provide path to your csv file.

### app.config

The application uses a ``gmail`` account to send the emails to your participants. You need to create your own version of app.config file into the path ``src/app.config`` with the following schema and your gmail address and password. I haven't provided app.config file to avoid accidentally committing my account info to the repository.

```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <appSettings>
    <add key="Sender Email" value="email@email.com" />
    <add key="Sender Email Password" value="pass" />
  </appSettings>
</configuration>
```

**NOTE:** Gmail will not let you actually connect to the email unless you allow less secure apps to access your Gmail account from your security settings.

## How to build

When opening this repo as workspace in VS Code it will prompt the recommended extensions for this workspace. Those are documented in [.vscode/extensions.json](.vscode/extensions.json)

Run commands:

```powershell
dotnet tool restore # Downloads used dotnet tools, ie. paket manager, source .config/dotnet-tools.json
dotnet build        # Builds app
```

These will build app into path ``src/bin/debug/netcoreapp3.0/SecretSanta.exe``

## How to run

The application has the following command line arguments:

```
USAGE: SecretSanta.exe [--help] --participants-path <path>

OPTIONS:

    --path, -p <Participants Path>
                          Specify the path to the participants csv file
    --help                display this list of options.
```

You can execute the app either by building it and starting the .exe file with path argument (path to your participants csv file.) or with the commands:

```powershell
cd src
dotnet run -- -p <path to participants file> # If using relative path the app is run from src/bin/debug/netcoreapp3.0
```

## Output

The app shuffles names in participants file and generates the pairing. Then it sends everyone an email with subject ``Your secret santa recipient`` and html content where Person1 is the name of gift giver and Person2 the recipient's name.

```html
<pre>
                                  ________________________________________
                                 /                                        \
             ______________      | Dear Person1,                          |
            /         \    \     |                                        |
           /           \    \    | Your secret santa gift recipient is    |
          / _________   \    |   | Person2!                               |
          \/     _   \  /    |   |                                        |
           \ /    \   \/_    |   | Please remember this is a secret,      |
           |  O_O_/   || \_  \   | so hus hus... ;)                       |
          / __(_ __   ||   \ /   |                                        |
         /\/___\___\_/  \  /_\   | Merry Christmas!                       |
        /      __        \/   \  | Your Secret Santa Bot                  |
        |                |\___/ /_________________________________________/
  ______|________        |  \   
 /              /\       /   \
 |              | \     /     \
 \______________\  |___/       \
  _|___            |_____\      \
 /|  __|           |_|| o \      \
| |    \           |_||____\_____/\
\_|____/     /     |        \_____/____
   |        //     |         /     \   \
   |       //      |_________\_/\   \   \__  ______
   |      /        |         /_ |_/_/      \/_//__/|
   |               |________// \|  \      //_//__/||\_
   |               |        \__//   |    / | ||__|/_  \
   |               |        |       |    \_|_/         \
   |  ___________  | |      |       /                   |
    \               \|______|      /              _____ |
     \               \      |      |      \\     _/   / |
      \               \_____|______\___    \    -/___/  |
       \                         ______\_____          /
        \                       / \          \        /
         \__________________    \ |           |______/
                            \    /            /
                             \__/____________/</pre>
```