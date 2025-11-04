# Changelog

## [1.4.5](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/compare/v1.4.4...v1.4.5) (2025-11-04)


### Code Refactoring

* **ShipsDataBaseModel:** update Stations property type ([c214204](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/c214204bf110dce32c8f1b6cf647b63ec5bc6539))
* **ShipsDataTransactionsModel:** update transaction handling ([c214204](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/c214204bf110dce32c8f1b6cf647b63ec5bc6539))
* **ShipsGraphsBaseModel:** update Stations property type ([c214204](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/c214204bf110dce32c8f1b6cf647b63ec5bc6539))
* **Station:** change StationList to ObservableCollection ([c214204](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/c214204bf110dce32c8f1b6cf647b63ec5bc6539))
* **StatsShipsLoadBaseModel:** update Stations property type ([c214204](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/c214204bf110dce32c8f1b6cf647b63ec5bc6539))
* **StatsShipsWaresBaseModel:** update Stations property type ([c214204](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/c214204bf110dce32c8f1b6cf647b63ec5bc6539))
* **StatsWaresShipsBaseModel:** update Stations property type ([c214204](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/c214204bf110dce32c8f1b6cf647b63ec5bc6539))


### Documentation

* **bbcode:** Update bbcode files ([abcafdd](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/abcafdd7b4f0f91f681981624ae026f5dd7c8463))
* **README:** update change log for version 1.4.5 ([996d8d6](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/996d8d6c196a77409187e19d25d74ed170e5f3df))

## [1.4.4](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/compare/v1.4.3...v1.4.4) (2025-10-25)


### Bug Fixes

* **DialogService:** update dialog colors for consistency ([8e6a8d6](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/8e6a8d61cbeeac7d75d13b179f178ab263e9940c))
* **MainWindow:** pass theme colors to ProgressWindow ([8e6a8d6](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/8e6a8d61cbeeac7d75d13b179f178ab263e9940c))
* **MainWindow:** remove binding converters for subordinate count display ([ce7dff7](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/ce7dff77ab6f06ae8152e83e0ee22bafbe08079e))
* **MainWindow:** remove subordinate count check from stats validation ([33a3feb](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/33a3febb0401b6edbd2bd733d6b9713fcf3c899e))
* **ProgressWindow:** bind theme colors and shadows ([8e6a8d6](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/8e6a8d61cbeeac7d75d13b179f178ab263e9940c))
* **ProgressWindow:** refactor constructor overloads for clarity ([93b0ed6](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/93b0ed651c1a0315fe77434997d2d9cb9c00ab75))
* **ProgressWindow:** update background and foreground colors for consistency ([78138cf](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/78138cf5ad0b4f576d20315329de332096e0af56))
* **README:** update change log for version 1.4.4 ([3ce4686](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/3ce4686fb9a4dba83da929f4eedda1fb23b41f57))


### Documentation

* **bbcode:** Update bbcode files ([dfc0de0](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/dfc0de0963ada7969205c502aa0f0166470b6278))
* **README:** update change log for version 1.4.4 ([9e742f6](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/9e742f66877d9fd61df49a2f01a8e080df95d8a8))

## [1.4.3](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/compare/v1.4.2...v1.4.3) (2025-10-24)


### Code Refactoring

* **ConfigurationViewModel:** add debug logging for game data reload ([0f981a7](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/0f981a74515e44307d2355372765f9fb02869d54))
* **ConfigurationViewModel:** add debug logging for save data reload ([fa2e3c1](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/fa2e3c1dbf147a988e980be76dbef3736e47e412))
* Enhance MainWindow and ProgressWindow for improved data initialization and user feedback ([5f16e44](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/5f16e44773835cb03dc10a9b8b1aafcd645c853e))
* **GameData:** add detailed logging for game XML load process ([f604e7f](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/f604e7fbc8a365af8c04c37da0e4a6af49e05bc3))
* **GameData:** add ship types tracking, processing and filtering ([bdd7bf2](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/bdd7bf22fddd723f954a70b0ddf0e49fa00df20c))
* **GameData:** call RefreshStats after vacuum operation ([0f981a7](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/0f981a74515e44307d2355372765f9fb02869d54))
* **GameData:** change gameTime and time types from int to long ([8d98a49](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/8d98a492064b11c01af6a58fe5bf680e7383b098))
* **GameData:** improve regex for ship type parsing ([5e815c2](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/5e815c2efbe56c5182293e8bb562491648d9e193))
* **GameData:** remove unused clearGameData flag ([b1787b1](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/b1787b1839d15c34b9d6c367f51a96249a1df048))
* **logging:** add logging service with configurable options ([042ee56](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/042ee565ba643196b07334ca854d3d7228d0ecbf))
* **MainViewModel:** add debug logging for data loading process ([7468719](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/7468719b9a0274cbef9a2462837c36660012e3ec))
* **MainViewModel:** enhance data loading with progress updates ([0f981a7](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/0f981a74515e44307d2355372765f9fb02869d54))
* **MainWindow:** add debug logging for data reload processes ([0f981a7](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/0f981a74515e44307d2355372765f9fb02869d54))
* **StatsShipsLoadBaseModel:** ensure UI thread access during reload ([0f981a7](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/0f981a74515e44307d2355372765f9fb02869d54))
* **StatsShipsWaresBaseModel:** ensure UI thread access during reload ([0f981a7](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/0f981a74515e44307d2355372765f9fb02869d54))
* **StatsWaresShipsBaseModel:** ensure UI thread access during reload ([0f981a7](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/0f981a74515e44307d2355372765f9fb02869d54))


### Documentation

* **bbcode:** Update bbcode files ([bf708ed](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/bf708ed0668e95b005ab6170fa7ea200b1237892))
* **README:** clarify issue description for scrap metal collector ([5978a79](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/5978a7968d4e41776fad84b89d86363971ad26d8))
* **README:** update change log for version 1.4.3 ([e8b73df](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/e8b73df6d67be8054dee108ae085e22a263405dd))

## [1.4.2](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/compare/v1.4.1...v1.4.2) (2025-10-20)


### Bug Fixes

* **GameData:** update database schema version and view creation ([5956890](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/5956890b3308da972324b11a2db3d0db717a9b5f))
* **Transaction:** handle invalid records during transaction addition ([5956890](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/5956890b3308da972324b11a2db3d0db717a9b5f))


### Documentation

* **bbcode:** Update bbcode files ([e2f3a3f](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/e2f3a3fff406fcbe3ab05c92a1c4858f15b1fdd0))
* **README:** update change log for version 1.4.2 ([f731bb1](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/f731bb1b737aeb024f5a6db34e358bfb08e7a0e9))
* **README:** update scrap metal capacity reporting ([f731bb1](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/f731bb1b737aeb024f5a6db34e358bfb08e7a0e9))

## [1.4.1](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/compare/v1.4.0...v1.4.1) (2025-10-20)


### Bug Fixes

* **database:** reorder index creation for ship_storage table ([667f44b](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/667f44b5765c7a83efd954ee49c92ffae375a57b))


### Documentation

* **bbcode:** Update bbcode files ([3a3a4e4](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/3a3a4e452afcaecd123a84163edd613a9db8f8ae))
* **README:** update cargo capacity utilization section ([b6ff8fe](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/b6ff8fee793fab6df259bd4d584f2b1a3fe17af9))
* **README:** update change log for version 1.4.1 ([4f0fdc1](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/4f0fdc1d9617d68163ae0dfe6a7ddc389d08d8d9))

## [1.4.0](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/compare/v1.3.1...v1.4.0) (2025-10-20)


### Features

* Add distance tracking to trades and implement sector connections ([2f38239](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/2f3823970ca78ff72a02afa35bab25c1d592a70b))
* **AutoReloadGameSaveMode:** define enum for auto-reload modes ([466eb25](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/466eb2583d63ba375ad2a5a4068ea87bf5a805d3))
* **Component:** add Macro property ([81cdf13](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/81cdf1319e091a3810d310ecff6e90deea590635))
* **ConfigurationService:** add AutoReloadGameSaveMode property ([466eb25](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/466eb2583d63ba375ad2a5a4068ea87bf5a805d3))
* **ConfigurationService:** add CheckForUpdatesOnStartup property ([9bbd774](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/9bbd7742b3361d1b666fe3f89940aecb97aaca57))
* **ConfigurationViewModel:** implement auto-reload functionality ([466eb25](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/466eb2583d63ba375ad2a5a4068ea87bf5a805d3))
* **ConfigurationViewModel:** include storage counts in configuration ([786af38](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/786af38ef9590d8dd9a06d6f03d77f03c37164f5))
* **ConfigurationViewModel:** integrate update checking logic ([9bbd774](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/9bbd7742b3361d1b666fe3f89940aecb97aaca57))
* **DialogService:** implement dialog methods for user notifications ([9bbd774](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/9bbd7742b3361d1b666fe3f89940aecb97aaca57))
* **FullTrade:** add Distance property and update calculations ([1191bca](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/1191bca7b464a2f607682bac61eb9dd2650cfc58))
* **FullTrade:** add LoadPercent calculation for trade data ([44c5006](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/44c5006f1463cb6ff0acee0241cd7e4e7b6f11d9))
* **FullTrade:** add MaxQuantity property ([81cdf13](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/81cdf1319e091a3810d310ecff6e90deea590635))
* **FullTrade:** add ShipClass property for trade normalization ([47f0fc6](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/47f0fc6fb11d713f50997f3b634f52b9bf72ff27))
* **GameData:** add storage and ship storage management ([786af38](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/786af38ef9590d8dd9a06d6f03d77f03c37164f5))
* **GameData:** add SubordinateCount property and related logic ([a4a8121](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/a4a8121dc02a80b70e71df5d02cf14421fc4a046))
* **GameData:** implement superhighway database schema ([f605e4e](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/f605e4ee5ffde3f0ed6074019917b7afb89183a5))
* **GameData:** integrate subordinate processing logic ([ab3ad17](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/ab3ad1784f83e3f343262159a40b53097be0b3f4))
* **GameData:** update database schema version to 5 ([0deca77](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/0deca77bc1d9745f1d910ed5f1e6cca90163a4cc))
* **GraphBucketItem:** create model for graph bucket items ([728812a](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/728812a23055a8da1aa14f1852ae30655581fee2))
* **LoadPercentPalette:** create utility for load percent colors ([728812a](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/728812a23055a8da1aa14f1852ae30655581fee2))
* **LoadPercentToBrushConverter:** implement converter for LoadPercent color coding ([44c5006](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/44c5006f1463cb6ff0acee0241cd7e4e7b6f11d9))
* **LoadPercentToBrushConverter:** refactor brush selection logic ([728812a](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/728812a23055a8da1aa14f1852ae30655581fee2))
* **MainViewModel:** add load statistics for ships and trades ([728812a](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/728812a23055a8da1aa14f1852ae30655581fee2))
* **MainWindow.axaml:** display Distance in DataGrid ([1191bca](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/1191bca7b464a2f607682bac61eb9dd2650cfc58))
* **MainWindow:** add UI checkbox for ReverseSort option ([2aa5a9d](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/2aa5a9d4005d2e8873cd1e67806d7c61a080de11))
* **MainWindow:** add UI elements for update information ([9bbd774](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/9bbd7742b3361d1b666fe3f89940aecb97aaca57))
* **MainWindow:** add UI for ships load distribution ([728812a](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/728812a23055a8da1aa14f1852ae30655581fee2))
* **MainWindow:** display LoadPercent in data grid ([44c5006](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/44c5006f1463cb6ff0acee0241cd7e4e7b6f11d9))
* **MainWindow:** display storage counts ([786af38](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/786af38ef9590d8dd9a06d6f03d77f03c37164f5))
* **MainWindow:** update UI for auto-reload settings ([466eb25](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/466eb2583d63ba375ad2a5a4068ea87bf5a805d3))
* **ProgressUpdate:** extend progress tracking ([786af38](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/786af38ef9590d8dd9a06d6f03d77f03c37164f5))
* **ProgressUpdate:** track superhighways processed in progress updates ([f605e4e](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/f605e4ee5ffde3f0ed6074019917b7afb89183a5))
* **ProgressWindow:** show processing status for storages ([786af38](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/786af38ef9590d8dd9a06d6f03d77f03c37164f5))
* **Sector:** add support for superhighway connections ([f605e4e](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/f605e4ee5ffde3f0ed6074019917b7afb89183a5))
* **Ship:** add MaxQuantity and LoadPercent properties ([44c5006](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/44c5006f1463cb6ff0acee0241cd7e4e7b6f11d9))
* **Ship:** introduce ShipClassFilter and ShipClassFilterUtil ([47f0fc6](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/47f0fc6fb11d713f50997f3b634f52b9bf72ff27))
* **ShipsDataBaseModel:** implement ship class filtering ([47f0fc6](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/47f0fc6fb11d713f50997f3b634f52b9bf72ff27))
* **ShipsDataTradesModel:** bind Distance property to trades ([1191bca](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/1191bca7b464a2f607682bac61eb9dd2650cfc58))
* **ShipsDataTradesModel:** filter trades by selected ship class ([47f0fc6](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/47f0fc6fb11d713f50997f3b634f52b9bf72ff27))
* **ShipsDataTransactionsModel:** filter transactions by selected ship class ([47f0fc6](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/47f0fc6fb11d713f50997f3b634f52b9bf72ff27))
* **ShipsDataTransactionsModel:** include MaxQuantity and LoadPercent in transactions ([44c5006](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/44c5006f1463cb6ff0acee0241cd7e4e7b6f11d9))
* **ShipsGraphsBaseModel:** add ship class filtering capability ([47f0fc6](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/47f0fc6fb11d713f50997f3b634f52b9bf72ff27))
* **ShipsGraphTradesModel:** filter graph data by selected ship class ([47f0fc6](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/47f0fc6fb11d713f50997f3b634f52b9bf72ff27))
* **StatsShipsLoadBaseModel:** add Sections for highlighting selected ship ([91fdc63](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/91fdc636ef141700bec0a3bb58ccc77c2156ba72))
* **StatsShipsLoadBaseModel:** implement base model for load statistics ([728812a](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/728812a23055a8da1aa14f1852ae30655581fee2))
* **StatsShipsLoadBaseModel:** implement ship class filtering ([47f0fc6](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/47f0fc6fb11d713f50997f3b634f52b9bf72ff27))
* **StatsShipsLoadTradesModel:** filter load trades by selected ship class ([47f0fc6](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/47f0fc6fb11d713f50997f3b634f52b9bf72ff27))
* **StatsShipsLoadTradesModel:** implement trades load statistics ([728812a](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/728812a23055a8da1aa14f1852ae30655581fee2))
* **StatsShipsLoadTransactionsModel:** filter load transactions by selected ship class ([47f0fc6](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/47f0fc6fb11d713f50997f3b634f52b9bf72ff27))
* **StatsShipsLoadTransactionsModel:** implement transactions load statistics ([728812a](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/728812a23055a8da1aa14f1852ae30655581fee2))
* **StatsShipsWaresBaseModel:** add ReverseSort property for sorting options ([2aa5a9d](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/2aa5a9d4005d2e8873cd1e67806d7c61a080de11))
* **StatsShipsWaresBaseModel:** add ship class filtering ([47f0fc6](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/47f0fc6fb11d713f50997f3b634f52b9bf72ff27))
* **StatsWaresBaseModel:** add Sections for highlighting selected ware ([91fdc63](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/91fdc636ef141700bec0a3bb58ccc77c2156ba72))
* **StatsWaresShipsBaseModel:** add ReverseSort property for sorting options ([2aa5a9d](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/2aa5a9d4005d2e8873cd1e67806d7c61a080de11))
* **StatsWaresShipsBaseModel:** add Sections for highlighting selected ship ([91fdc63](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/91fdc636ef141700bec0a3bb58ccc77c2156ba72))
* **StatsWaresShipsBaseModel:** implement ship class filtering ([47f0fc6](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/47f0fc6fb11d713f50997f3b634f52b9bf72ff27))
* **StatsWaresShipsTradesModel:** filter trades by selected ship class ([47f0fc6](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/47f0fc6fb11d713f50997f3b634f52b9bf72ff27))
* **StatsWaresShipsTransactionsModel:** filter transactions by selected ship class ([47f0fc6](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/47f0fc6fb11d713f50997f3b634f52b9bf72ff27))
* **Subordinate:** add Subordinate class for managing subordinate data ([ab3ad17](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/ab3ad1784f83e3f343262159a40b53097be0b3f4))
* **Transaction:** add MaxQuantity property ([81cdf13](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/81cdf1319e091a3810d310ecff6e90deea590635))
* **Transaction:** include ShipClass in transaction data ([47f0fc6](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/47f0fc6fb11d713f50997f3b634f52b9bf72ff27))
* **UpdateChecker:** add functionality to check for updates from GitHub ([9bbd774](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/9bbd7742b3361d1b666fe3f89940aecb97aaca57))


### Bug Fixes

* **GameData:** add storage module XML file retrieval ([ff80d05](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/ff80d05e7f017d5605c20b54ae6dddb817d4ebac))
* **GameData:** restrict buyer and seller joins to stations ([a0ca73d](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/a0ca73d3358a8b0bc3e2c5fca814264583c2a06d))
* **MainViewModel:** add ApplyThemeOnCharts method and invoke on window open ([c418ac2](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/c418ac2029122562b143389ea4878f033fcf975f))
* **MainViewModel:** pass version to ConfigurationViewModel ([9bbd774](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/9bbd7742b3361d1b666fe3f89940aecb97aaca57))
* **MainWindow.axaml:** update SortMemberPath for DataGridTemplateColumns ([7a39109](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/7a391099622c6fbcb3781828207451209e54e170))
* **MainWindow:** bind Sections to CartesianChart ([91fdc63](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/91fdc636ef141700bec0a3bb58ccc77c2156ba72))
* **ProgressWindow:** update UI bindings for superhighways processed ([f605e4e](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/f605e4ee5ffde3f0ed6074019917b7afb89183a5))
* **ShipsGraphsBaseModel:** modify constructor to accept theme colors ([c418ac2](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/c418ac2029122562b143389ea4878f033fcf975f))
* **Transaction:** clear sector connection caches on transaction clear ([f605e4e](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/f605e4ee5ffde3f0ed6074019917b7afb89183a5))


### Code Refactoring

* **ChartPalette:** add PickForLong method ([803568d](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/803568d3605ad6135f251ed2b035bcc3ff6896c6))
* Consolidate and rename view models for ships and wares statistics ([a290a05](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/a290a05c4d1e69de2dead4ac2bc1aa1b4f9ca6e4))
* **GameData:** extract ProcessStationOrShip method for clarity ([9e56b4c](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/9e56b4c5733255f1b4abdc4e5df4e4a7d3b062b1))
* **GameData:** remove unused TryReadLanguageAttribute method ([46292a4](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/46292a433e5810aa8288967e14d39bcd2937c3f0))
* **GameData:** streamline station name detection logic ([e758c0d](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/e758c0d97fb8680bc35e5419dd0f7939a16133cc))
* **GameData:** update database schema for storage and component ([81cdf13](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/81cdf1319e091a3810d310ecff6e90deea590635))
* **MainViewModel:** remove unused WaresStats properties ([4efdef3](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/4efdef3ad9c84dbe0e287bb81be27deda2b6aeec))
* **MainWindow:** remove Wares Stats tab ([4efdef3](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/4efdef3ad9c84dbe0e287bb81be27deda2b6aeec))
* **Ship:** change ShipId type from int to long ([803568d](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/803568d3605ad6135f251ed2b035bcc3ff6896c6))
* **ShipsDataTransactionsModel:** change ShipId type from int to long ([803568d](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/803568d3605ad6135f251ed2b035bcc3ff6896c6))
* **ShipsGraphsBaseModel:** change ShipId type from int to long ([803568d](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/803568d3605ad6135f251ed2b035bcc3ff6896c6))
* **ShipsGraphTradesModel:** change ShipId type from int to long ([803568d](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/803568d3605ad6135f251ed2b035bcc3ff6896c6))
* **Station:** streamline station list management ([01dd838](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/01dd838b74a543dfe322ab02dff1761092e8002f))
* **StatsShipsLoadBaseModel:** change ShipId type from int to long ([803568d](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/803568d3605ad6135f251ed2b035bcc3ff6896c6))
* **StatsShipsLoadTradesModel:** change ShipId type from int to long ([803568d](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/803568d3605ad6135f251ed2b035bcc3ff6896c6))
* **StatsShipsLoadTransactionsModel:** change ShipId type from int to long ([803568d](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/803568d3605ad6135f251ed2b035bcc3ff6896c6))
* **StatsShipsWaresBaseModel:** change ShipId type from int to long ([803568d](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/803568d3605ad6135f251ed2b035bcc3ff6896c6))
* **StatsShipsWaresBaseModel:** optimize ship index validation ([ca69d3a](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/ca69d3aceb612c2ca6267c73c7ee306e15c15c48))
* **StatsShipsWaresTradesModel:** change ShipId type from int to long ([803568d](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/803568d3605ad6135f251ed2b035bcc3ff6896c6))
* **StatsShipsWaresTransactionsModel:** change ShipId type from int to long ([803568d](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/803568d3605ad6135f251ed2b035bcc3ff6896c6))
* **StatsWaresShipsBaseModel:** change ShipId type from int to long ([803568d](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/803568d3605ad6135f251ed2b035bcc3ff6896c6))
* **StatsWaresShipsBaseModel:** optimize ship index validation ([ca69d3a](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/ca69d3aceb612c2ca6267c73c7ee306e15c15c48))
* **StatsWaresShipsTradesModel:** change ShipId type from int to long ([803568d](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/803568d3605ad6135f251ed2b035bcc3ff6896c6))
* **StatsWaresShipsTransactionsModel:** change ShipId type from int to long ([803568d](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/803568d3605ad6135f251ed2b035bcc3ff6896c6))
* **Transaction:** change ShipId type from int to long ([803568d](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/803568d3605ad6135f251ed2b035bcc3ff6896c6))
* **WaresStatsBaseModel:** delete unused base model ([4efdef3](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/4efdef3ad9c84dbe0e287bb81be27deda2b6aeec))
* **WaresStatsTradesModel:** delete unused trades model ([4efdef3](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/4efdef3ad9c84dbe0e287bb81be27deda2b6aeec))
* **WaresStatsTransactionsModel:** delete unused transactions model ([4efdef3](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/4efdef3ad9c84dbe0e287bb81be27deda2b6aeec))


### Documentation

* **bbcode:** Update bbcode files ([fa80454](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/fa804541cce7d404b5b850f321123b28dbf938e1))
* **bbcode:** Update bbcode files ([c27b9e6](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/c27b9e604673ed1c527ab28ada39d9979baa25e0))
* **README:** add new features link demo for version 1.4.0 ([bbe48ff](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/bbe48ffac1453478c4ac285d80821ae9c17d0032))
* **README:** add update checker and auto-reload features ([776e9b2](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/776e9b2e11f8ef06d9f3ea16e90b763ab04f3a3a))
* **README:** update change log for version 1.4.0 ([7d1fa2b](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/7d1fa2bfbd1f4d0ea4d237837df94aa8bde2d265))

## [1.3.1](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/compare/v1.3.0...v1.3.1) (2025-09-09)


### Code Refactoring

* **Converters:** add BoolToChartPositionConverter for tooltip and legend positioning ([4461a1e](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/4461a1e0319ea9809558b36deee2dd5b38c57808))
* **Models:** add GraphShipItem and GraphWareItem classes ([4461a1e](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/4461a1e0319ea9809558b36deee2dd5b38c57808))
* **ViewModels:** enhance MainViewModel with chart registration ([4461a1e](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/4461a1e0319ea9809558b36deee2dd5b38c57808))
* **ViewModels:** enhance WaresShipsStatsBaseModel with new properties ([4461a1e](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/4461a1e0319ea9809558b36deee2dd5b38c57808))
* **ViewModels:** update ShipsGraphsBaseModel and ShipsWaresStatsBaseModel ([4461a1e](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/4461a1e0319ea9809558b36deee2dd5b38c57808))
* **Views:** enhance MainWindow.axaml.cs for chart functionality ([4461a1e](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/4461a1e0319ea9809558b36deee2dd5b38c57808))
* **Views:** update MainWindow.axaml for new UI elements ([4461a1e](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/4461a1e0319ea9809558b36deee2dd5b38c57808))


### Documentation

* **README:** update YouTube video link and add version 1.3.1 changes ([5089a96](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/5089a96aa3b34b0f56b1bf6407caceb5d736372f))

## [1.3.0](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/compare/v1.2.0...v1.3.0) (2025-09-08)


### Features

* **ConfigurationViewModel:** bind RemovedObjectCount to view model ([28cf6ca](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/28cf6ca8e24e0991445ca4a998e5b9d7bef9013b))
* **ContentExtractor:** enhance catalog initialization and file extraction ([12911e6](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/12911e68daa0106c3d91cc6edee1c80128c56a1d))
* **Converters:** add EnumEqualsConverter for enum binding ([7269a7f](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/7269a7fd17c1f175ea58a683ae4cc5aab4767f17))
* **Converters:** add ZeroToRedIfEnabledConverter for conditional coloring ([0965941](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/09659414610160fdbfa4416f88beadd4f473a988))
* **DlcResolver:** add DLC resolution logic ([b487d75](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/b487d7558e5bee5962ecb7d6db521ea133009829))
* **DlcResolver:** enhance DLC discovery and content ID handling ([57a3abd](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/57a3abddcc0eecedb360b176c0d4af3a11b26d57))
* **ExtensionResolver:** add extension resolution logic ([65ecfb0](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/65ecfb02158c6e80f40c39644d0c5eec7014388f))
* **FullTrade:** add SpentTime property for trade duration calculation ([a13bb6d](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/a13bb6d17cff7ee96526b2ae57179d93fe67b7da))
* **FullTrade:** enhance trade duration representation ([32d3bcb](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/32d3bcbd184a4d8f42e036457fffcd6cfe7b7da6))
* **FullTrade:** introduce FullTrade and TradeLeg models ([246e157](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/246e157763426d81aa5b1d9e8dde8902b915eeed))
* **FullTradesModel:** add summary fields for trade statistics ([32d3bcb](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/32d3bcbd184a4d8f42e036457fffcd6cfe7b7da6))
* **FullTradesModel:** implement data loading and refreshing logic ([246e157](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/246e157763426d81aa5b1d9e8dde8902b915eeed))
* **FullTradesModel:** implement ship filtering and trade steps rebuilding ([a13bb6d](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/a13bb6d17cff7ee96526b2ae57179d93fe67b7da))
* **GameData:** add GetComponentById method for component retrieval ([3916be1](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/3916be1b46ccf90621763c7ff122e6a7002fd87e))
* **GameData:** add GetFactionsShortNamesDict method ([4059c67](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/4059c6793031e3f906014b8239532b51d8366cbe))
* **GameData:** add GetFullTrades method for detecting full trades ([246e157](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/246e157763426d81aa5b1d9e8dde8902b915eeed))
* **GameData:** add handling for removed entries in XML processing ([3147cd9](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/3147cd99810c8c8d664e89bd6d3a914df2f3969e))
* **GameData:** add RemovedObjectCount property and update stats ([28cf6ca](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/28cf6ca8e24e0991445ca4a998e5b9d7bef9013b))
* **GameData:** improve full trade detection ([32d3bcb](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/32d3bcbd184a4d8f42e036457fffcd6cfe7b7da6))
* **GameData:** include sector information in trade queries ([a13bb6d](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/a13bb6d17cff7ee96526b2ae57179d93fe67b7da))
* **GameData:** update faction name handling in data processing ([4059c67](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/4059c6793031e3f906014b8239532b51d8366cbe))
* **MainViewModel:** add ShipsTransactionsShipsWaresStats property ([54632d7](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/54632d7bae1deb9283f65b072dbdabc9af655005))
* **MainViewModel:** add TransactionsWaresShipsStats and TradesWaresShipsStats properties ([2b7a93a](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/2b7a93a078c334998405ee6e5d583c9c61f6595b))
* **MainViewModel:** integrate FullTradesModel for UI updates ([246e157](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/246e157763426d81aa5b1d9e8dde8902b915eeed))
* **MainWindow.axaml:** restructure tabs for better organization ([8e32bda](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/8e32bda9fa910b28b7ce37b3c5b6b5b1cd0a11a2))
* **MainWindow:** add Ships per Wares Stats tab ([54632d7](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/54632d7bae1deb9283f65b072dbdabc9af655005))
* **MainWindow:** add Ships Trades tab with data grid for trades ([a13bb6d](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/a13bb6d17cff7ee96526b2ae57179d93fe67b7da))
* **MainWindow:** display RemovedObjectCount in UI ([28cf6ca](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/28cf6ca8e24e0991445ca4a998e5b9d7bef9013b))
* **MainWindow:** enhance ship list display ([0e21bec](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/0e21beca467f1af784fd94ae302abf53545d68b6))
* **MainWindow:** enhance UI for ship sorting ([752012b](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/752012b6cb38e8956dbb66d4b5d33c1e63245c6d))
* **MainWindow:** update tab headers and add new tabs for Wares by Ships Stats ([2b7a93a](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/2b7a93a078c334998405ee6e5d583c9c61f6595b))
* **ProgressUpdate:** include removed entries count in progress updates ([3147cd9](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/3147cd99810c8c8d664e89bd6d3a914df2f3969e))
* **ProgressWindow:** display removed entries processed in UI ([3147cd9](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/3147cd99810c8c8d664e89bd6d3a914df2f3969e))
* **Services:** add LoadRemovedObjects property to ConfigurationService ([0965941](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/09659414610160fdbfa4416f88beadd4f473a988))
* **ShipsTransactionsModel:** add sorting functionality for ships ([752012b](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/752012b6cb38e8956dbb66d4b5d33c1e63245c6d))
* **ShipsTransactionsModel:** aggregate ship profits during transaction load ([0e21bec](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/0e21beca467f1af784fd94ae302abf53545d68b6))
* **ShipsWaresStatsBaseModel:** create base model for ships wares stats ([54632d7](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/54632d7bae1deb9283f65b072dbdabc9af655005))
* **ShipsWaresStatsTradesModel:** add ShipsWaresStatsTradesModel class ([938ea16](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/938ea16f256d546b30d9e0191e1419a88bb5bf83))
* **ShipsWaresStatsTransactionsModel:** implement transactions model for ships wares stats ([54632d7](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/54632d7bae1deb9283f65b072dbdabc9af655005))
* **TimeFormatter:** add utility for time formatting ([32d3bcb](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/32d3bcbd184a4d8f42e036457fffcd6cfe7b7da6))
* **ViewModels:** implement ship sorting functionality ([7269a7f](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/7269a7fd17c1f175ea58a683ae4cc5aab4767f17))
* **Views:** integrate LoadRemovedObjects into MainWindow ([0965941](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/09659414610160fdbfa4416f88beadd4f473a988))
* **WaresShipsStatsBaseModel:** create base model for Wares and Ships stats ([2b7a93a](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/2b7a93a078c334998405ee6e5d583c9c61f6595b))
* **WaresShipsStatsTradesModel:** implement trades statistics model ([2b7a93a](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/2b7a93a078c334998405ee6e5d583c9c61f6595b))
* **WaresShipsStatsTransactionsModel:** implement transactions statistics model ([2b7a93a](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/2b7a93a078c334998405ee6e5d583c9c61f6595b))
* **WaresStatsBaseModel:** add base model for ware statistics ([3ec032c](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/3ec032c213c8d511b375af7ec56e2ef59926a3ea))
* **WaresStatsTradesModel:** add WaresStatsTradesModel class ([dbf14a7](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/dbf14a740d2e7c3fb4b781fbb11846d5ea300dca))
* **WaresStatsTransactionsModel:** implement transactions model for ware stats ([3ec032c](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/3ec032c213c8d511b375af7ec56e2ef59926a3ea))


### Bug Fixes

* **GameData:** streamline removed entries processing ([3916be1](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/3916be1b46ccf90621763c7ff122e6a7002fd87e))
* **MainWindow:** enable transactionsWaresShipsStatsTab when data is ready ([f6e1172](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/f6e11727c67c14cf2a900e798374172fb52c5219))
* **MainWindow:** ensure Readme content is loaded on tab selection ([74eac85](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/74eac85d51cd0959f072d9813ccea55563769ff6))
* **MainWindow:** rename DataGrid column headers for clarity ([abb1f40](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/abb1f40a4cb913dde243d5d45beb207865004eb4))
* **MainWindow:** update data context binding for WaresStats ([3ec032c](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/3ec032c213c8d511b375af7ec56e2ef59926a3ea))
* **on-markdown-update:** add 'steam' conversion type for markdown processing ([b272d20](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/b272d20f7b03fcf7e32bbccb43a5b414d44c5fea))


### Code Refactoring

* **DlcResolver:** remove unused class ([65ecfb0](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/65ecfb02158c6e80f40c39644d0c5eec7014388f))
* **FullTrade:** change properties from init to set ([2e9a844](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/2e9a8440e6e27453834a02782be20fe61236d558))
* **FullTradesModel:** move ShipSortOrder enum to Ship.cs ([752012b](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/752012b6cb38e8956dbb66d4b5d33c1e63245c6d))
* **FullTradesModel:** moved GetFullTrades from the GameData ([696ddc6](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/696ddc6fd389c055c3d7ac21cc7a67140127396c))
* **FullTradesModel:** rename trade time properties ([696ddc6](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/696ddc6fd389c055c3d7ac21cc7a67140127396c))
* **FullTradesModel:** update trade handling logic ([2e9a844](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/2e9a8440e6e27453834a02782be20fe61236d558))
* **FullTrade:** update GetFullTrades method to accept allTransactions ([f7b90da](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/f7b90da8cf6a25aef452f3baeb74eec54876825d))
* **GameData:** delegate DLC resolution to DlcResolver ([b487d75](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/b487d7558e5bee5962ecb7d6db521ea133009829))
* **GameData:** simplify id  parsing logic - comply with removed objects ids ([bbd1698](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/bbd16982214d2e59989d569096b0bc75fc9d3f2b))
* **GameData:** update database schema version to 2 ([2d01f1e](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/2d01f1ecef2ce7bc2dfa6623acfebb051c285c3a))
* **GameData:** update DLC handling to extensions ([65ecfb0](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/65ecfb02158c6e80f40c39644d0c5eec7014388f))
* **MainViewModel:** manage transactions centrally ([3e80369](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/3e8036935e39de4a355816f97cfeeff3500e96cd))
* **MainViewModel:** refresh additional transactions stats ([69644e0](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/69644e0f9c30ded917dcf06d6051679f4daa6cb4))
* **MainViewModel:** rename properties for clarity ([dbf14a7](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/dbf14a740d2e7c3fb4b781fbb11846d5ea300dca))
* **MainViewModel:** rename transaction-related properties for clarity ([938ea16](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/938ea16f256d546b30d9e0191e1419a88bb5bf83))
* **MainViewModel:** rename WaresStats to ShipsWaresStats ([3ec032c](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/3ec032c213c8d511b375af7ec56e2ef59926a3ea))
* **MainViewModel:** replace Trades with static AllTrades list ([e386975](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/e386975dae3a9037fe76bee706b1c4621a34f330))
* **MainViewModel:** update LoadData method to pass allTransactions ([f7b90da](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/f7b90da8cf6a25aef452f3baeb74eec54876825d))
* **MainWindow.axaml.cs:** add new tab item references ([dbf14a7](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/dbf14a740d2e7c3fb4b781fbb11846d5ea300dca))
* **MainWindow.axaml:** update data context bindings ([dbf14a7](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/dbf14a740d2e7c3fb4b781fbb11846d5ea300dca))
* **MainWindow.axaml:** update tab item headers for consistency ([84e707c](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/84e707c889d0d21d6f39fe63887daf03f54dbaef))
* **MainWindow:** add suggested start locations for file picker on Windows ([db32e93](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/db32e937d31a67dc46396293f1b8113e25956324))
* **MainWindow:** replace DataGridTextColumn with DataGridTemplateColumn for Time ([4d1e236](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/4d1e236ab2a185d71ceb365e9f02c271a9b3d339))
* **MainWindow:** set initial folder based on a current values for file/folder pickers ([1fcc6a0](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/1fcc6a012c60988e9947200e6a5ccf102cac9922))
* **ShipsDataBaseModel:** consolidate summary fields and properties ([d1f08c4](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/d1f08c40632d3d3990309137a58cb2cb398fe5db))
* **ShipsDataTradesModel:** remove redundant summary fields ([d1f08c4](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/d1f08c40632d3d3990309137a58cb2cb398fe5db))
* **ShipsDataTradesModel:** update ship name handling ([f7b90da](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/f7b90da8cf6a25aef452f3baeb74eec54876825d))
* **ShipsDataTradesModel:** use MainViewModel.AllTrades instead of local list ([e386975](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/e386975dae3a9037fe76bee706b1c4621a34f330))
* **ShipsDataTransactionsModel:** improve transaction loading logic ([448e592](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/448e5921ddaf22559e5e1b1d6b3bd36dfc9e3dcc))
* **ShipsDataTransactionsModel:** simplify ship sorting logic ([83a5ce6](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/83a5ce6541326f3094c5a21e2000e5d88a5d31fe))
* **ShipsDataTransactionsModel:** streamline transaction loading ([3e80369](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/3e8036935e39de4a355816f97cfeeff3500e96cd))
* **ShipsDataTransactionsModel:** streamline transport type filters ([d1f08c4](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/d1f08c40632d3d3990309137a58cb2cb398fe5db))
* **ShipsGraphsBaseModel:** update color retrieval logic ([d1f08c4](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/d1f08c40632d3d3990309137a58cb2cb398fe5db))
* **ShipsGraphsTransactionsModel:** enhance ship graph data loading ([3e80369](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/3e8036935e39de4a355816f97cfeeff3500e96cd))
* **ShipsGraphsTransactionsModel:** simplify transport type checks ([d1f08c4](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/d1f08c40632d3d3990309137a58cb2cb398fe5db))
* **ShipsGraphTradesModel:** optimize trade filtering and cumulative profit calculation ([448e592](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/448e5921ddaf22559e5e1b1d6b3bd36dfc9e3dcc))
* **ShipsGraphTradesModel:** update grouping to use ShipFullName ([f7b90da](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/f7b90da8cf6a25aef452f3baeb74eec54876825d))
* **ShipsGraphTradesModel:** utilize MainViewModel.AllTrades for trade data ([e386975](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/e386975dae3a9037fe76bee706b1c4621a34f330))
* **ShipsGraphTransactionsModel:** streamline ship sorting process ([83a5ce6](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/83a5ce6541326f3094c5a21e2000e5d88a5d31fe))
* **ShipsTransactionsModel:** rename filter methods for clarity ([e14c248](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/e14c248a11333b202e9796880aeecb091a1c32f8))
* **ShipsTransactionsModel:** rename transaction time properties ([696ddc6](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/696ddc6fd389c055c3d7ac21cc7a67140127396c))
* **Transaction:** add StationOwner and StationCode properties ([f7b90da](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/f7b90da8cf6a25aef452f3baeb74eec54876825d))
* **Transaction:** add Transaction model for trade operations ([3e80369](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/3e8036935e39de4a355816f97cfeeff3500e96cd))
* **Transaction:** update Time property to use TimeFormatter ([448e592](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/448e5921ddaf22559e5e1b1d6b3bd36dfc9e3dcc))
* **WaresStatsTradesModel:** streamline data loading for ware statistics ([448e592](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/448e5921ddaf22559e5e1b1d6b3bd36dfc9e3dcc))
* **WaresStatsTradesModel:** switch to MainViewModel.AllTrades for data filtering ([e386975](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/e386975dae3a9037fe76bee706b1c4621a34f330))
* **WaresStatsTradesModel:** update grouping to use Ware and Product ([f7b90da](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/f7b90da8cf6a25aef452f3baeb74eec54876825d))
* **WaresStatsTransactionsModel:** optimize ware profit calculation ([3e80369](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/3e8036935e39de4a355816f97cfeeff3500e96cd))


### Documentation

* **bbcode:** Update bbcode files ([0519d53](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/0519d539d47cd4e32b9e4513c8aade2185a909b6))
* **bbcode:** Update bbcode files ([a3e0ec8](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/a3e0ec8db16b74427fb2c4283c63123dbe2e9f29))
* **bbcode:** Update bbcode files ([075ac45](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/075ac456738a586506c519b52d71a595ad5b7fb8))
* **README.md:** correct date and grammar in change log ([670c751](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/670c751d55ea14a4ef1f6fe68d5ce63b8b78ce60))
* **README:** enhance feature descriptions and add new analysis modes ([11bb10d](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/11bb10da515134f404d624169ab5e06f6453b6b8))
* **README:** update reset dialog behavior for game paths ([de59aca](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/de59acabcb9995e40b14e8b0fa4bdae09aff6441))
* **README:** update YouTube video link for app demonstration ([58fc461](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/58fc461cb73fd239b6e60e2781dfba2e08dad3cb))

## [1.2.0](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/compare/v1.1.3...v1.2.0) (2025-09-04)


### Features

* **GameData:** add GetWareComponentNamesDict method for ware component mapping ([e91c28f](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/e91c28f9c576fb648b9b903e11efa557af14b02d))
* **GameData:** implement database schema versioning and version 1 - ware table extended by component_macro field ([d8dbc0c](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/d8dbc0c1b22b983570d03dbda376f3e106ed105a))


### Bug Fixes

* **GameData:** clear trade and component tables in case of schema update ([5d14b2f](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/5d14b2f36467924f70e4995da9d919310bca6c84))
* **GameData:** station with docked player ships was skipped ([162fd50](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/162fd501d6a3d134f46085ba588b5152bd4070e0))
* **GameData:** update progress tracking for processed stations and trades ([94bb136](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/94bb1362a41cd406e3fb8cd970d8d0843a86ccce))


### Code Refactoring

* **Component:** introduce GameComponent model ([162fd50](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/162fd501d6a3d134f46085ba588b5152bd4070e0))


### Documentation

* **README.md:** add change log for version 2.0.0 ([e2ee9b9](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/e2ee9b93efd96d051da90f730e7e1f0b135a2b5c))
* **README.md:** update installation instructions for Linux support ([e2ee9b9](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/e2ee9b93efd96d051da90f730e7e1f0b135a2b5c))

## [1.1.3](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/compare/v1.1.2...v1.1.3) (2025-09-04)


### Miscellaneous Chores

* release 1.1.3 to reflect ci fixes ([504e05e](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/504e05e19e2ff84c64ac491d2762f4574ba845c3))


### Documentation

* **bbcode:** Update bbcode files ([be301b5](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/be301b59cc3cfa6234337d508e0923d973bf5bc7))
* **readme:** update change log for version 1.1.3 ([8cabbc1](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/8cabbc17fa243798c4dd09a7b954b0959a5c61fb))

## [1.1.2](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/compare/v1.1.1...v1.1.2) (2025-09-04)


### Bug Fixes

* **GameData:** handle malformed production entries gracefully ([3ac4af3](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/3ac4af3a70e15d9ad5d9a66f3caec4770addba89))


### Documentation

* **bbcode:** Update bbcode files ([2ddd721](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/2ddd721d2190950fde73fcbc482955f240a9cdb7))
* **README:** update change log for version 1.1.2 ([ff8bd66](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/ff8bd66afdf750c40ae85364d3799f0915fd00cf))

## [1.1.1](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/compare/v1.1.0...v1.1.1) (2025-09-04)


### Features

* **images:** add new images for documentation ([220a494](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/220a494befcd18798efa2697bc86a638906b0fcd))


### Bug Fixes

* **GameData:** enhance error handling for production and component entries ([77c2c53](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/77c2c53099f04f030fdb84224fe41def48124d0c))
* **GameData:** improve trade entry parsing and error handling ([3dd57c4](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/3dd57c4ee96f4c2d7bda9bdef2ed60e1972e703f))
* **GameData:** refactor trade processing logic for clarity and accuracy ([5a16cb3](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/5a16cb3b694143c3babd8eb56370002bbe5d0d42))
* **MarkdownViewer:** recreate MarkdownViewer on theme change as workaround of issue with code block rendering ([d2955c4](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/d2955c4b48e131c5bdc1e4f7ffc525165447cc35))


### Miscellaneous Chores

* release 1.1.1 to reflect only fixes ([4a88f46](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/4a88f46a4450384cbe6c4b96d13dd0db6354b98a))


### Documentation

* **bbcode:** Update bbcode files ([a09e803](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/a09e80391adedc63784e91af2f6e22aebf6e68fe))
* **README:** add change log for version updates ([f54a386](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/f54a386a254b4f23059f178d380a555482d51b0a))
* **README:** update important notice about trade logs ([e9901bf](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/e9901bffeb849f757c46c9f1ef6d7c99439dd579))
* Update images ([180ac28](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/180ac282e17f4c01cd23856c5c63cdcd104e202d))

## [1.1.0](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/compare/v1.0.0...v1.1.0) (2025-09-03)


### Features

* **ConfigurationViewModel:** include cluster sector names in readiness check ([569813d](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/569813d01327876b9a71c11399abdc4f99354278))
* **GameData:** enhance data handling for cluster sectors ([569813d](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/569813d01327876b9a71c11399abdc4f99354278))
* **images:** add title360 image ([493cde1](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/493cde193cf880df7f4af61be54744819a06dc1d))
* **MainWindow:** display cluster sector names count ([569813d](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/569813d01327876b9a71c11399abdc4f99354278))
* **ProgressUpdate:** extend progress tracking metrics ([569813d](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/569813d01327876b9a71c11399abdc4f99354278))
* **ProgressWindow:** add cluster sector names processing display ([569813d](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/569813d01327876b9a71c11399abdc4f99354278))
* **Ship:** add Sector property to ShipTransaction class ([569813d](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/569813d01327876b9a71c11399abdc4f99354278))
* **ShipsTransactionsModel:** update SQL query to include sector ([569813d](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/569813d01327876b9a71c11399abdc4f99354278))


### Miscellaneous Chores

* **images:** update logo.ico file ([87b3bfe](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/87b3bfe1f1bb30bb7c40523cedc286325a3b6040))
* **src/Directory.Build.props:** create new Directory.Build.props file ([8f74b47](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/8f74b47a0e7bf2a911f9c434b2221b990ec8a271))


### Documentation

* **bbcode:** Update bbcode files ([312dc06](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/312dc06cb928392a0f4ac054ffdd00abb734068b))
* **images:** add new image files for documentation ([7663950](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/7663950401a1b90220f973f0651d38b1f5b6d398))
* **README.md:** add acknowledgements section ([bad73de](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/bad73deab5e105a172a33616eb87268cdc3688eb))
* **README.md:** fix last line ([286defa](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/286defa11cd9f1a574561dd08f7efbe33be979a8))

## [1.0.0](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/compare/v0.1.0...v1.0.0) (2025-09-03)


### ⚠ BREAKING CHANGES

* first public version

### Features

* first public version ([eaeedd8](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/eaeedd8feea96d885cb014078be5bf437e71b748))
* **MainWindow:** update UI to display trip time metrics ([e6b8552](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/e6b85520b0012d8fea4e4f1beff87b3527cfb91a))
* **ShipsTransactionsModel:** add trip time calculations ([e6b8552](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/e6b85520b0012d8fea4e4f1beff87b3527cfb91a))


### Bug Fixes

* **ConfigurationService:** simplify JsonSerializerOptions initialization ([9ec2a4a](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/9ec2a4a97a6389a10fc47ec9d1c48985298c5854))
* **MainWindow.axaml:** adjust indentation for theme selector and statistics table ([cf3fb78](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/cf3fb783ece00d076014b73aa46f8b4fa1c5ba64))
* **MainWindow:** enhance file picker for executable and save game ([9ec2a4a](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/9ec2a4a97a6389a10fc47ec9d1c48985298c5854))
* **MainWindow:** format binding for numeric values to make them sortable ([9974e99](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/9974e99179ece78cf739840c80d1e3ec1cdf48ab))


### Code Refactoring

* **ShipsTransactionsModel:** adjust data type conversions ([9974e99](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/9974e99179ece78cf739840c80d1e3ec1cdf48ab))
* **ShipTransaction:** update property types for consistency ([9974e99](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/9974e99179ece78cf739840c80d1e3ec1cdf48ab))


### Miscellaneous Chores

* **workspace:** initialize workspace file ([a9bed56](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/a9bed5617bc3b3ac508b0b6d134094b8a2c4ba3d))


### Documentation

* add preview images ([57a3edc](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/57a3edc4b755816844c1004fdc63a13a7939f47a))
* add title and thumbnails ([d2ee744](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/d2ee7443fe6c5716d7a2df2b0ddf34368676e146))
* **bbcode:** Update bbcode files ([7fa70be](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/7fa70beec03e552d1a574552abdb5a30a8856427))
* **bbcode:** Update bbcode files ([32b8c68](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/32b8c68c5c7bb75b763cb5541da530bcc3a07eeb))
* **bbcode:** Update bbcode files ([9c3e677](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/9c3e677c1784fafdf2fd7c32e18e02db42efcf2c))
* **bbcode:** Update bbcode files ([a879ef2](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/a879ef21d5df17d73d759e747718e6ab9ba2c7fe))
* **bbcode:** Update bbcode files ([201a207](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/201a2077bdd76aa3819c8b17a471867b52307d8c))
* first graph screenshot ([362da6a](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/362da6ae0f9b861128440ac5525c739f64f265e8))
* first screenshot ([fa38d51](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/fa38d51f631de0ee53ebbf3e13fa1c5599e796a2))
* **README.md:** update download instructions and configuration sections for clarity ([1b524e5](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/1b524e5337f18f571ad99201cda0a8d7025ee48b))
* **readme:** clarify tool's purpose and add credit to X4MagicTripLogAnalyzer ([870880b](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/870880b6f97d9390bf3e15e9ceec9434cc2b278e))
* **readme:** update project name to X4PlayerShipTradeAnalyzer ([a9bed56](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/a9bed5617bc3b3ac508b0b6d134094b8a2c4ba3d))
* **readme:** update project release link and clarify execution steps ([6e80610](https://github.com/chemodun/X4PlayerShipTradeAnalyzer/commit/6e80610988a953010fbbc1dbba029a96bcafcba3))
