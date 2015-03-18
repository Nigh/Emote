#NekoHacks
##E-mote DX9 Sample
###Project AZUSA

## 编译提示 ##
环境：VS 2013，DX 9.0c
首先要在项目的包含目录和库目录中加入DX9的相关目录。
如果你要加载的PSB立绘来自nekopara，则将EMOTE_NEKOPARA设为1。

## 说明 ##
由于E-mote SDK经常出现变动，要显示你想要采用的立绘，需要从来源处取得emotedriver.dll。不同版本的emotedriver通常不通用。

本示例默认使用的版本为官方为YU-RIS引擎提供的DX版SDK中的emotedriver，但也提供nekopara版本的emotedriver.dll。但**Project AZUSA不提供关于nekopara的问题的解释与回答。**

为方便切换版本，我们将YU-RIS版emotedriver重命名为yurisdriver.dll。nekopara版本保留原名emotedriver.dll。

nekopara版本的使用方法是由 百度贴吧@霍普菲尔德 研发的，对他的深入研究表示感谢！

同时，nekopara版本dll经由 @9chu 进行过一次Patch，修复了原dll在绘图调用上的一个BUG，该BUG的影响未知，但至少会导致无法正常进行图形调试。

由于nekopara立绘均在40MB以上，且出于其他因素考虑，本源代码不包含nekopara立绘文件。

## LICENSE ##
CC 3.0 BY-NC-SA


----------
by Ulysses

![署名-非商业性使用-相同方式共享 3.0 中国大陆](http://i.creativecommons.org/l/by-nc-sa/3.0/88x31.png)