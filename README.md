## 为什么会有这个工具

  坚持了半年commit，突然有几天没空提交代码，感觉太烦躁了，这工具就这么出来了

## 原理

  github统计小绿点的逻辑 : [Why are my contributions not showing up on my profile?](https://help.github.com/articles/why-are-my-contributions-not-showing-up-on-my-profile/)
  能通过[github的api](https://api.github.com/users/liaozixu/events)看到，我是个经常删项目的人（不要问我为什么），发现项目删除后，小绿点也消失了。拿着公司的git，创建了项目，直接同步了过来。发现之前的小绿点亮了！
  so
  那我修改时间，不同时间段commit一份不就好了吗？！
然后我就开始开干了...

## 使用方法

  项目基于c#，只在window10上测试过，客户端需要安装git的client。一晚时间赶出来，可能会有很多的bug。
  下载项目代码，丢Visual Studio里面开build，就能用了。
  如果不想麻烦，可以直接[点击这里下载](https://github.com/liaozixu/githubGreen-csharp/blob/master/githubGreen/obj/Release/githubGreen.exe)
  需要现在先在git创建仓库，再在本地创建一个临时文件夹，工具会自动clone一份下来，并且根据start time和end time，修改电脑时间，修改文件和commit，最后push。
  
  ![软件界面](https://static.zixu.hk/liaozixu.com/cms/upload/20180805/235106/28478faf20744d3c8e00f9a51df41d19.png)
  
  在clone url填写项目地址，在项目首页的Clone or download处复制，使用clone with https的地址。
  temp需要建立一个空白文件夹，选中。
  orderliness和random的区别是，每天提交一次和每天随机次数提交的意思[随机值为1-20，可以在代码的loopingCreateCommit方法中修改]
  git exe path 填写 git.exe 的位置
  `注意：不是git-bash.exe或者git-cmd.exe，在git官方下载安装的话，他就叫git.exe`
  点击start开刷[期间可能需要github账号密码，会有弹窗（建议安装github desktop，不安装这个好像有时候登录框出不来，导致工具假死）]

## 工具安全性
  代码全开源，可以自己检查后编译使用。
  基本就是Process调git...
  
  
## 最后
  开发时全网搜索了一遍，其实发现这个小工具已经有人做出来了...
  也有参考了下他的代码[@ahangchen](https://github.com/ahangchen/green)
  python就是好啊，代码简洁看起来爽
