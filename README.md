## LucaSystemTools
Prototype's galgame tools
## 文件

很多图像相关的提取代码都是[deqxj00](https://github.com/wetor/LucaSystemTools/commits?author=deqxj00) 写的，脚本的话都是我写的，CZ0和Pak和一些代码来自[LucaSystem](https://github.com/marcussacana/LucaSystem)。

图像结构相关解释可以等贴吧@DeQxJ00发帖

与air相同引擎的游戏脚本相关，可等贴吧@develseed发帖

NS两款游戏脚本相关，看代码就行了~

**PakTools.cs**

- 此引擎的pak包解包工具

**CZ0Parser.cs、CZ1Parser.cs、CZ3Parser.cs、CZ4Parser.cs**

- Prototype新游戏好像用的都是这种图片，dat的升级版，CZ1提取和打包完成，CZ3只有提取， 新增CZ4

**DatParser.cs**

- Psv air的几乎所有图像的提取程序，同引擎适用。具体打包请参照 [PSV AIR 汉化工具](https://github.com/YuriSizuku/GalgameReverse/blob/master/prototype/prot_dat.py)

**FontInfoParser.cs**

- island字体的info文件解析，同引擎适用，DeQxJ00整的 [CZ1和info解析](https://tieba.baidu.com/p/6033002424)

**PsbScript.cs**

- air脚本的简单解析，psv的clannad应该同样适用，其中A3 A4结尾的为跳转指令，不写了，详情 [PSV AIR 汉化工具](https://github.com/YuriSizuku/GalgameReverse/blob/master/prototype/airpsv_text.py)

**ScriptOpcode.cs**

- opcode类，主要是用来解析已知参数的指令  

**ScriptParser.cs**

- 脚本初步反汇编，支持修改然后汇编回去，文本增长什么的测试无问题（JUMP、GOTO等跳转指令未实际测试）  

***Support Game***
>- 《Summer Pocket》Nintendo Switch  反编译、编译  
>- 《Clannad》Nintendo Switch  反编译、编译（Opcode未完善）  
>- 《Tomoyo After Its a Wonderful Life CS Edition》Nintendo Switch  反编译、编译  
>- 《ISLAND》Psvita  反编译、编译  

## 能做什么

- 做NS版的Summer Pocket 、Tomoyo After Its a Wonderful Life完整汉化没问题了，NS版的Clannad的话要改下opcode的解释。

- psv上和air同期的prototype的游戏，只要文件结构相似，那么图像基本是都能提取的，

- 大体上psv的clannad air rewrite 是一类 ，psv上的 island flowers系列等是一类 ，如遇到同引擎的一些游戏可以参考下

- 支持psv的ISLAND脚本编译与反编译
