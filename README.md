# My.ps2DLC

## My.ps2DLC.DLL に埋め込むリソース

`Architecture: amd64`、`LocaleName: ja-JP` 環境で `PS2DLC.Enable()` が使用するファイルは合計 18 ファイルです。
リソース実体は `My.ps2DLC.DLL/Resources/Original/` 配下に配置し、ビルド時に `My.ps2DLC.DLL.dll` へ埋め込みます。

### 選択条件

| 種別 | 選択条件 | 件数 |
| --- | --- | ---: |
| `Binaries` | `My.ps2DLC.DLL/Resources/Original/Binaries/amd64/` 配下の `*.dll` | 9 |
| `ResourceBinaries` | `My.ps2DLC.DLL/Resources/Original/ResourceBinaries/amd64/ja-JP/` 配下の `*.dll` | 7 |
| `regkeysNew` | スクリプト内で指定されている 2 つの `.reg` ファイル | 2 |

### Binaries

```text
My.ps2DLC.DLL/Resources/Original/Binaries/amd64/Microsoft.PowerShell.Commands.Diagnostics.dll
My.ps2DLC.DLL/Resources/Original/Binaries/amd64/Microsoft.PowerShell.Commands.Management.dll
My.ps2DLC.DLL/Resources/Original/Binaries/amd64/Microsoft.PowerShell.Commands.Utility.dll
My.ps2DLC.DLL/Resources/Original/Binaries/amd64/Microsoft.PowerShell.ConsoleHost.dll
My.ps2DLC.DLL/Resources/Original/Binaries/amd64/Microsoft.PowerShell.Security.dll
My.ps2DLC.DLL/Resources/Original/Binaries/amd64/Microsoft.WSMan.Management.dll
My.ps2DLC.DLL/Resources/Original/Binaries/amd64/Microsoft.WSMan.Runtime.dll
My.ps2DLC.DLL/Resources/Original/Binaries/amd64/System.Management.Automation.dll
My.ps2DLC.DLL/Resources/Original/Binaries/amd64/pspluginwkr.dll
```

### ResourceBinaries

```text
My.ps2DLC.DLL/Resources/Original/ResourceBinaries/amd64/ja-JP/Microsoft.PowerShell.Commands.Diagnostics.resources.dll
My.ps2DLC.DLL/Resources/Original/ResourceBinaries/amd64/ja-JP/Microsoft.PowerShell.Commands.Management.resources.dll
My.ps2DLC.DLL/Resources/Original/ResourceBinaries/amd64/ja-JP/Microsoft.PowerShell.Commands.Utility.resources.dll
My.ps2DLC.DLL/Resources/Original/ResourceBinaries/amd64/ja-JP/Microsoft.PowerShell.ConsoleHost.resources.dll
My.ps2DLC.DLL/Resources/Original/ResourceBinaries/amd64/ja-JP/Microsoft.PowerShell.Security.resources.dll
My.ps2DLC.DLL/Resources/Original/ResourceBinaries/amd64/ja-JP/Microsoft.WSMan.Management.resources.dll
My.ps2DLC.DLL/Resources/Original/ResourceBinaries/amd64/ja-JP/System.Management.Automation.resources.dll
```

### regkeysNew

```text
My.ps2DLC.DLL/Resources/Original/regkeysNew/regkeyPowerShell.reg
My.ps2DLC.DLL/Resources/Original/regkeysNew/regkeyPowerShellEngine.reg
```
