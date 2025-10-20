# X4 Player Ship Trade Analyzer

![Title](https://raw.githubusercontent.com/chemodun/X4PlayerShipTradeAnalyzer/refs/heads/main/docs/images/title360.jpg)

A simple, fast desktop tool for X4: Foundations players to understand their ship's trading performance. Point it at your game folder and a save file and get clear insights into:

- Which wares make you the most profit
- How each of your ships has been trading over time
- Totals for price, quantity, and estimated profit per transaction

**Important notice**: Please be aware - trade logs are collected by game for AI pilots only.

The app runs locally and reads only your X4 files. Nothing is uploaded. **Nothing is MODDED!**

## Key features

- Two analysis modes:
  - By Transactions: Each buy/sell action is analyzed separately. This is faster and shows more data, but profit is estimated based on average prices.
  - By Trades: Profit is calculated based on actual trades (buy+sell). This method is more accurate for profit calculation, but may show fewer entries if some ships haven't completed trades yet.
- Ships transactions/trades: A detailed table of every transaction/trade in your save
  - Columns for Transactions: Time, Sector, Station, Operation (Buy/Sell), Product, Price, Quantity, Total, Estimated Profit
  - Columns for Trades: Time, Product, Bought, Sold, Profit, Spent Time.
    - Columns for related Transactions: Time, Operation, Volume, Price, Sector, Station
- Transactions filters for ware type: Container, Solid, Liquid, Gas
- Trades filter: With Internal Trades (i.e. trades between your stations can be excluded)
- Ships graphs: Compare ships visually
  - Interactive chart per ship; double-click a ship in the list to toggle it on/off
  - Same Container/Solid/Liquid/Gas filters can be applied
- Wares stats: See what actually makes money
  - Single-series histogram with per-ware colors and tooltips
  - Custom legend shows color + ware name for quick scanning
- Ships by Wares: See which ships trade which wares
  - Stacked column chart with ships on X-axis and wares colored
  - Sorted by profit in descending order
  - Same Container/Solid/Liquid/Gas filters can be applied
- Wares by Ships: See which wares are traded by which ships
  - Same as above, but with wares on X-axis and ships colored
  - Sorted by profit in descending order
  - Same Container/Solid/Liquid/Gas filters can be applied
- Configuration (first-run setup and updates from save files)
  - Set Game Folder (your X4.exe location)
  - Choose a save file (.xml.gz)
  - Optional theme: System, Light, or Dark
  - Update checker: view installed version, run a manual check, or enable automatic checks on startup
  - Quick stats to confirm data loaded (wares, factions, ships, stations, trades, language)
- Built-in Readme: A “Readme” tab mirrors this guide inside the app

## Download and run

1) Get the app
   - Recommended: Download the latest Windows or Linux build from the [Nexus Mods](https://www.nexusmods.com/):
      - [Chem O`Dun](https://next.nexusmods.com/profile/ChemODun/mods?gameId=2659) → [X4 Player Ship Trade Analyzer](https://www.nexusmods.com/x4foundations/mods/1801)

2) Install and run
   - Windows:
     - Unzip the downloaded `X4PlayerShipTradeAnalyzer-win_?.?.?.zip` and run `X4PlayerShipTradeAnalyzer.exe` inside the folder `X4PlayerShipTradeAnalyzer`.
   - Linux:
     - Unzip `X4PlayerShipTradeAnalyzer-linux_?.?.?.zip` and then extract `X4PlayerShipTradeAnalyzer` folder from `X4PlayerShipTradeAnalyzer-linux_?.?.?.tar.gz`.
     - Run executable `X4PlayerShipTradeAnalyzer`.

## First-time setup (Configuration tab)

1) Game Folder
   - Click Set next to “Game Folder” and select your X4.exe. Typical paths for Windows:
     - Steam: `C:\Program Files (x86)\Steam\steamapps\common\X4 Foundations\X4.exe`
     - GOG/Epic: wherever you installed X4

2) Save file
   - Click Set next to “Game Save Path” and choose a save `.xml.gz`. Typical path for Windows:
     - Steam: `%USERPROFILE%\Documents\Egosoft\X4\<player-id>\save\quicksave.xml.gz`
     - GOG/Epic: `%USERPROFILE%\Documents\Egosoft\X4\save\quicksave.xml.gz`

3) Load data
   - Click “Reload Data” for the Game Folder to import base game data (wares, factions, etc.).
   - Click “Reload Data” for the Save Path to import your transactions.
   - When data is present, the other tabs become enabled.

4) Optional settings
   - Load Only Game Language: speeds up loading by using your game language only.
   - Theme: System, Light, or Dark.

5) Auto-reload
   - Enable “Auto-reload game saves" radio button:
   - with "No": the app will not monitor changes to your save file. You will need to manually reload data when you want to see new transactions.
     - with "Selected Save file": the app will monitor changes to your selected save file and automatically reload data when it changes on disk. As selected save file will be used latest loaded one.
     - with "Any Save file": the app will monitor changes to any save file in your X4 saves folder and automatically reload data when any of them changes on disk. The folder is determined based on latest loaded save file.

6) Update checker
   - Check for updates on startup: automatically runs the release check when the analyzer opens.
   - Use the “Check for updates” button at the bottom to run the check immediately.

## Using the app

- By Transactions
  - Ships transactions tab
    - Browse trades with totals and estimated profit.
    - Filter by:
      - Parent Station: Any/None or Exact one.
      - Ship Class: All, XL, L, M, S.
      - Cargo type: Container/Solid/Liquid/Gas
    - Can be sorted by Ship Name/Profit in a Ships List.

  - Ships graphs tab
    - Visualize activity and compare ships.
    - **Double-click a ship** in the list to show/hide it on the chart.
    - Toggled Ship in a List will be have colored similar to the chart.
    - Filter by:
      - Parent Station: Any/None or Exact one.
      - Ship Class: All, XL, L, M, S.
      - Cargo type: Container/Solid/Liquid/Gas
    - Can be sorted by Name/Profit in a Ships List.

  - Cargo capacity utilization distribution across all transactions tab
    - Show histogram of cargo capacity utilization (%) for all transactions.
    - Filter by:
      - Parent Station: Any/None or Exact one.
      - Ship Class: All, XL, L, M, S.
      - Cargo type: Container/Solid/Liquid/Gas
    - Limit Top: 10, 25, 50, 100
    - Reverse order toggle.

  - Ships by Wares tab
    - Show Stacked Column chart of ships trading different wares.
    - Sorted by Profit in descending order.
    - Wares are colored.
    - Filter by:
      - Parent Station: Any/None or Exact one.
      - Ship Class: All, XL, L, M, S.
      - Cargo type: Container/Solid/Liquid/Gas
    - Limit Top: 10, 25, 50, 100
    - Reverse order toggle.

  - Wares by Ships tab
    - Show Stacked Column chart of wares traded by different ships.
    - Sorted by Profit in descending order.
    - Ships are colored.
    - Filter by:
      - Parent Station: Any/None or Exact one.
      - Ship Class: All, XL, L, M, S.
      - Cargo type: Container/Solid/Liquid/Gas
    - Limit Top: 10, 25, 50, 100
    - Reverse order toggle.

- By Trades
  - Same as above, but profit is calculated based on actual trades (buy+sell) rather than individual transactions.
  - This method is more accurate for profit calculation, but may show fewer entries if some ships haven't completed trades yet.
  - For filtering is used:
    - Parent Station: Any/None or Exact one.
    - Ship Class: All, XL, L, M, S.
    - With or without Internal Trades (i.e. trades between your stations can be excluded).
  - Top limit and Reverse order options are also available.

- Configuration tab
  - Change Game Folder or Save file and reload data.
  - Optional settings: Load Only Game Language/Load removed objects, Theme (System, Light, Dark).
  - Quick stats to confirm data loaded (wares, factions, ships, stations, trades, language).
  - Auto-reload data from saves when they change on disk.
  - Update checker section: shows the current app version, the last remote version detected, a “Check for updates” button, and a “Check for updates on startup” toggle.

- Readme tab
  - Shows this guide inside the app for quick reference.

## Video

- There is short video available on [YouTube](https://www.youtube.com/watch?v=z1xCXBDdpFk) demonstrating the app's features and usage.
- Stats Diagrams UI improvement in version 1.3.1: [YouTube](https://www.youtube.com/watch?v=NqAwX8Rl4Dc).
- New features in version 1.4.0: [YouTube](https://www.youtube.com/watch?v=Qxu8uV9mHsU).

## Tips & troubleshooting

- I see zeros / tabs are disabled
  - Make sure you set both Game Folder (X4.exe) and a valid save `.xml.gz`, then press both Reload buttons.

- Save not found
  - Check `%USERPROFILE%\Documents\Egosoft\X4\<player-id>\save\` for `quicksave.xml.gz` or any `*.xml.gz` save.

- Loading takes a while
  - Large saves can take a minute. A small progress window appears during import.

- Nothing uploads anywhere
  - The app reads your local files only and keeps all analysis on your machine.

## Change log

- 1.4.0 (2025-10-20)
  - Introduced:
    - New type of graphs: Cargo capacity utilization distribution across all transactions.
    - Filtering by Parent Station and Ship Class in all analysis tabs.
    - Auto-reload data from saves when they change on disk.
    - Update checker.
- 1.3.1 (2025-09-09)
  - Improved:
    - Stats Diagrams UI: replacement for the tooltip is implemented as a custom "legend" on a left side of the charts.
- 1.3.0 (2025-09-08)
  - Introduced:
    - Total profit summary in all Ships Lists
    - Sorting by Name/Profit in all Ships Lists
    - Processing more cargo types (i.e. liquids and gas added)
    - If removed objects (ships/stations) has a next component, it will be loaded too
    - Option to load removed objects (ships/stations) in Configuration tab
    - All analysis tabs now shown in two variants: By Transactions and By Trades
    - New Graphs tabs: Ships by Wares and Wares by Ships
  - Fixed:
    - For non-container wares average price was excluded from calculation
    - Multiple other small fixes and improvements
    - If game and save paths are set, resetting dialogs will start from those paths
- 1.2.0 (2025-09-05)
  - Fixed:
    - Station wasn't loaded if has Player ships docked
    - Ships names not loaded, if player ships weren't renamed
  - Introduced:
    - Database schema updates
  - Warnings:
    - This is a breaking change. Will force to make re-import your game data and save file after updating.
- 1.1.3 (2025-09-04)
  - Fixed:
    - Linux executable building
- 1.1.2 (2025-09-04)
  - Fixed:
    - Issue with stations load, introduced in 1.1.1
- 1.1.1 (2025-09-04)
  - Improved:
    - Speed of game save loading
  - Fixed:
    - Issue with data loading on certain save files (additional check is now performed)
    - Issue with displaying README after changing the Theme (issue on an element level)
- 1.1.0 (2025-09-03)
  - Introduced:
    - Added a Station Sector column in Ships Transactions table
- 1.0.0 (2025-09-02)
  - Introduced:
    - Initial release

## Credits

- Author: Chem O`Dun
- Based on idea implemented in [X4MagicTripLogAnalyzer by Magic Trip](https://github.com/magictripgames/X4MagicTripLogAnalyzer)
- Not affiliated with Egosoft. "X4: Foundations" is a trademark of its respective owner.

## Acknowledgements

- Thanks to all members of the [X4 modding channel](https://discord.com/channels/337098290917146624/502057640877228042) on [Egosoft Discord](https://discord.com/invite/zhs8sRpd3m). And especially to `UniTrader` and `Forleyor`.
- Thanks to [u/Breakfast-Excellent](https://www.reddit.com/user/Breakfast-Excellent/) for the report the trade loading issue and save file for testing.
