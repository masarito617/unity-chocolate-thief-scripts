# unity-chocolate-thief-scripts
「怪盗チョコレート」ゲームアプリ（※スクリプト部分のみ）<br>
バレンタイン用に作った、育成しながらチョコをひたすら集めるゲームです。<br>
- iOS<br>
https://apps.apple.com/jp/app/%E6%80%AA%E7%9B%97%E3%83%81%E3%83%A7%E3%82%B3%E3%83%AC%E3%83%BC%E3%83%88-3d%E3%83%81%E3%83%A7%E3%82%B3%E9%9B%86%E3%82%81%E3%82%A2%E3%82%AF%E3%82%B7%E3%83%A7%E3%83%B3/id1669439500
- Android<br>
https://play.google.com/store/apps/details?id=com.molegoro.chocolate
<img src="https://user-images.githubusercontent.com/77447256/218259846-baa1d0e1-31f6-4fa7-a146-139dffab6985.png" width="480px">
<img src="https://user-images.githubusercontent.com/77447256/218259893-a7ee2072-3016-49f0-89bf-8207ca06aaf2.png" width="480px">
<img src="https://user-images.githubusercontent.com/77447256/218259909-6a8d9151-b618-4306-8563-1a163935dc08.png" width="480px">

### 改訂履歴
- v1.2.0</br>・結果画面にツイートボタンを追加
- v1.1.0</br>・移動処理の際に範囲外にはみ出ることがあったため修正<br>
・ステータス割り振りによる強化度を変更<br>
・EXPを使用することでボーナスキャラの出現率がアップする機能を追加
- v1.0.0</br>・新規リリース
### Unityバージョン<br>
- 2021.3.1f1<br>
### 使用アセット・パッケージ<br>
- VContainer<br>https://github.com/hadashiA/VContainer
- UniRx<br>https://github.com/neuecc/UniRx
- UniTask<br>https://github.com/Cysharp/UniTask
- DOTWeen<br>https://assetstore.unity.com/packages/tools/animation/dotween-hotween-v2-27676
- Google Mobile Ads Unity Plugin<br>https://github.com/googleads/googleads-mobile-unity/releases/tag/v7.2.0
- social-connector<br>https://github.com/anchan828/social-connector/tree/v0.2.9
- UnmaskForUGUI<br>https://github.com/mob-sakai/UnmaskForUGUI
- Procedural UI Image<br>https://assetstore.unity.com/packages/tools/gui/procedural-ui-image-52200
- Very Animation<br>https://assetstore.unity.com/packages/tools/animation/very-animation-96826?locale=ja-JP
- Ultimate Hypercasual Characters<br>https://assetstore.unity.com/packages/3d/characters/humanoids/ultimate-hypercasual-characters-43-bodytypes-202182
- Low Poly Japanese Housing Complex<br>https://assetstore.unity.com/packages/3d/environments/urban/low-poly-japanese-housing-complex-162993
### 使用フォント<br>
- にしき的フォント<br>https://umihotaru.work
### 全体構成
- シーン設計<br>
![image](https://user-images.githubusercontent.com/77447256/218261124-8c57d104-6f90-44aa-8092-faa3aa3ededf.png)
  - TitleScene<br>タイトルTOPと強化画面を行き来するシーン
  - GameScene<br>チョコを集めるゲームシーン
  - RewardScene<br>スタッフロール再生シーン
  - InterstitialScene<br>インタースティシャル広告再生シーン
  
- ソフトウェア設計（ざっくり）<br>
![image](https://user-images.githubusercontent.com/77447256/218260673-8e385120-d8de-4963-b01d-d81bfa1a37bf.png)
