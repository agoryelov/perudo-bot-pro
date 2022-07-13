# Perudo Bot Pro

## How to run locally

### Prerequisites
* Microsoft .NET 6
* (optional) Python 3.8+ to use Discord client
* (optional) Windows OS to use sound effects

### Running Perudo server
1. Clone this repository
2. Open solution in Visual Studio
2. Set PerudoBot.API as a Startup Project
3. (optional) Set database name in `/PerudoBot.Database/PerudoBotDbContext.cs`
4. Run the app

### Running Discord client
1. Clone this repository
2. Open `PerudoBot.Discord` in VS Code
3. Complete `.env.example`
4. (optional) Create python venv `python -m venv .venv`
5. (optional) Activate python venv `./.venv/Scripts/activate.bat`
6. Install requirements `pip install -r ./requirements.txt`
7. Run `python ./main.py`
