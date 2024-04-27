# コピペ管理アプリ
## 概要
- クリップボードを監視してコピーされたアイテムを保存。自動処理や検索などを行う。
- OpenAIのアカウント(APIキー)を持ってる場合はコピーされたアイテムに対して要約や箇条書き化などの処理が行える。
- 日頃の仕事で多くを占める「コピー」「ペースト」「フォルダを開く」といった作業を補助することを目的としたもの。
- コピぺについてのOutlook風な機能(検索フォルダや自動仕分けなど)の実装を目指すところ。
- WPFアプリケーション。 .Net8を使用。
- AIの処理などにはPythonを使用。
- まだ開発中。

## 機能
- まだ開発中なので出来上がったタイミングでまとめる。


## 実行方法
1. このリポジトリをgit cloneする。
2. Visual Studio 2022をインストールする。このフォルダにある`WpfApp1.sln`をダブルクリックしてVisualStudioを起動。ビルドする。
3. Python 3.11をインストール
- このアプリでは、処理の一部にPythonを使用しているので、Pythonをインストールする。
- Pythonをインストールしなくてもクリップボード監視などの基本機能は利用可能。
- Pythonを使用しない場合は設定画面で`PythonExecution`を0に設定する。有効にする場合は1を設定する。
Python3.12は後述のpip installでpycocotoolsのビルドが発生する。 
ビルドにはVisual C++ ビルドツールをインストールする必要があるなどの手間がかかるので3.11をインストールする。

4. Pythonモジュールのインストール
このフォルダにあるrequreiments.txtに記載のモジュールをインストールする.
```
pip install -r requreiments.txt
```
5. ClipboardApp.exeを起動する。

6. 設定メニュー
- PythonDLLPath: Pythonインストール先のフォルダの`python311.dll`を設定する。  
  例：C:\Users\user\AppData\Local\Programs\Python\python311.dll

- PythonExecution:0はPythonを用いた処理を行わない。1はPythonを用いた処理を行う。

