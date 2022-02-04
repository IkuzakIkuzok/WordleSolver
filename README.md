# WordleSolver

[Wordle](https://www.powerlanguage.co.uk/wordle/)のソルバー

## 動作要件

- .NET 5
- \>=Windows 2000 ([`SendMessage`](https://docs.microsoft.com/ja-jp/windows/win32/api/winuser/nf-winuser-sendmessage#requirements)使用のため)

## 使用方法

![image](https://user-images.githubusercontent.com/22995544/152046179-3e85bb42-d404-4c55-88f8-df23ff2b1f14.png)

1. ソルバーのアルゴリズムを選択する (詳細は後述; 途中からでも変更可)
2. 左側に表示されている単語をWordleに入力する
3. 結果を右側に入力する (⬜=1, 🟨=2, 🟩=3としてキーボードによる入力も可 (e.g. 🟨⬜🟨⬜🟩=21213))
4. OKを押すと次に入力すべき単語が表示される (候補数が0の場合を除く)

## アルゴリズム

ソルバーのアルゴリズムは，大きくハードモード用とハードじゃないモード用に分類される。
ハードモードはすでに得られている結果(含まれる文字(位置を含む)，含まれない文字)を完全に反映した単語のみを候補とする。
一方ハードじゃないモードでは残っている候補を可能な限り限定するための単語を選ぶため，正解にはなり得ないことが明らかになっている単語も選択される。

各モードの詳細は以下の通りである。

### Hard mode, use simplified score

ハードモード用，簡易スコアを使用する。

ハードモード用の簡易スコアは，文字の位置ごとに全候補単語中の文字出現頻度を求め，得られた頻度を基に文字ごとのスコアを計算するものである。

例えば，pares, place, sleep, spaceの4単語が候補として残っているとする。
このとき，2文字目についてaが1回(=1点)，lが2回(=2点)，pが1回(=1点)となる。
同様にすべての位置について文字の点数を決定する。
その後，単語ごとに文字のスコアの合計を計算する。
例えばparesでは2+1+1+2+1=7点となる。同様にplaceが10点，sleepが9点，spaceが9点となる。

さらに，文字の重複による重みを付けるために，含まれる文字種の数(重複なし文字列の長さ)を乗じる。
例えばsleepはs, l, e, pの4種類の文字からなるので，すでに計算した9点に4を乗じて36点となる。
pares, place, spaceはそれぞれ5種類の文字を含むため5を乗じてそれぞれ35, 50, 45点となる。

なお，この方法ではスコアの値が大きくなる場合があるので必要に応じて正規化を行う。

以上のようにして得られたスコアによって候補を降順にソートしたものが入力すべき単語の候補となる(この場合ではplace)。

### Hard mode, use entropy

ハードモード用，シャノンエントロピーを使用する。

単語それぞれについて得られる情報量を最大化するように候補を決定する。

ある単語
<img src="https://latex.codecogs.com/gif.latex?x" />
のエントロピー
<img src="https://latex.codecogs.com/gif.latex?H(x)" />
は

<img src="https://latex.codecogs.com/gif.latex?H(x)=-\sum_{\text{results}}p(\text{result})\log(\text{result})" />

と計算される。ただし，
<img src="https://latex.codecogs.com/gif.latex?p(\text{result})" />
は，それを正解と仮定して<img src="https://latex.codecogs.com/gif.latex?x" />を入力した際に結果がresultとなる単語の数を全候補数で割った値である。

以上のようにして得られたエントロピーによって候補を降順にソートしたものが入力すべき単語の候補となる。

なお，エントロピーの計算が重い(単語数<img src="https://latex.codecogs.com/gif.latex?n" />に対して
<img src="https://latex.codecogs.com/gif.latex?O(n)^2" />)ため並列化している。そのため候補の選択は決定的ではない(同じエントロピーの異なる単語が選択され得る)。

### Non-hard mode, use simplified score

ハードじゃないモード用，簡易スコアを使用する。

この場合の簡易スコアはハードモードの場合とは異なる計算方法になる。
ハードモードでは文字の位置ごとに点数を求めていたが，ハードじゃないモードではすべての位置で点数を合算する。
さらに，含まれていないことがわかっている文字については減点を行う。
文字ごとの点数の決定後の処理はハードモードと同様である。

例えば，以下のような場合を考える。

`TARES` 🟨⬜⬜⬜⬜<br>
`ULTIO` ⬜⬜🟨🟨⬜<br>
`FIGHT` ⬜🟩🟩🟩🟩<br>
`NYMPH` 🟩⬜⬜⬜🟨<br>
`NIGHT` 🟩🟩🟩🟩🟩<br>

4手目(NYMPH)の時点での候補はbight, dight, hight, might, night, wightの6単語である。
これらから正解を決定するためにはb, d, h, m, n, wの中のどの文字が含まれているかを決定すればよいので，これらの文字を多く含むnymphが選択される。

### Non-hard mode, use weighted entropy

ハードじゃないモード用，重み付きエントロピーを使用する。

全単語についてエントロピーを計算し，ハードじゃないモード用の簡易スコアを乗じて重み付けを行う。

## その他の機能

### 正規表現検索

[Tool] \> [Regex] または Ctrl+F

全単語の中から正規表現で単語を検索することができる。
Priorityにはメインウィンドウで選択したアルゴリズムに応じたスコアが表示される。
エントロピーを計算するアルゴリズムを選択している場合には検索に時間がかかることがある。

"Inherit filters from main form"にチェックを入れるとメインウィンドウで候補として残っている単語のみを検索対象とする。
ハードモード用の場合にはメインウィンドウを検索画面で同じ結果となるが，ハードじゃないモード用では異なる結果となる。

### シミュレーション

[Tool] \> [Simulate] または Ctrl+R

任意の語(単語一覧に含まれるものに限る)が正解である場合のソルバーの動作をシミュレートする。
