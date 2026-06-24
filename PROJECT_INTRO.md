# PowerToys Run Password Generator

> 为 PowerToys Run 打造的极速密码生成器 —— `Alt+Space` 即刻生成高强度密码

## 一句话介绍

**PowerToys Run Password Generator** 是一款专为 PowerToys Run 设计的密码生成插件，让你无需打开浏览器、无需登录账户，只需按下 `Alt+Space` 即可在 2 秒内生成安全可靠的随机密码。

## 为什么做这个插件？

每次需要密码时，你是不是也这样：
- ❌ 打开浏览器 → 搜索 "password generator" → 等待网页加载 → 可能被广告干扰
- ❌ 打开密码管理器 → 等待同步 → 找到生成器功能 → 多步操作
- ❌ 手动编造密码 → 不够随机 → 安全性存疑

**这个插件解决的问题：**
- ✅ 零摩擦：Alt+Space → 输入 `pw` → 回车，密码已在剪贴板
- ✅ 零依赖：无需网络、无需账户、无需订阅
- ✅ 高安全：FIPS 140-2 认证 CSPRNG，超越主流密码管理器的安全等级
- ✅ 本地运行：密码从未离开你的电脑

## 技术亮点

| 特性 | 实现方式 | 优势 |
|------|---------|------|
| 随机数生成 | Windows CNG (BCryptGenRandom) | FIPS 140-2 认证，硬件级熵源 |
| 内存安全 | `char[]` + 显式清零 | 防止密码残留内存 |
| 架构设计 | 单例 SessionState | 会话级密码缓存，进程退出即清除 |
| 零网络 | 纯本地实现 | 无隐私泄露风险 |

## 适用场景

- 快速生成临时密码
- 创建 WiFi 密码（易读、无相似字符）
- 生成 PIN 码
- 需要高强度密码的任何时刻

## 与主流方案对比

| 方案 | 步骤数 | 需要网络 | 需要账户 | 开源 |
|------|--------|---------|---------|------|
| LastPass 生成器 | 3-4 | ✅ | ✅ | ❌ |
| Bitwarden 生成器 | 3-4 | ✅ | ✅ | ✅ |
| **本插件** | **2** | ❌ | ❌ | ✅ |

## 开始使用

```bash
# 安装
1. 下载 Release 中的 PasswordGenerator.zip
2. 解压到 %LOCALAPPDATA%\Microsoft\PowerToys\PowerToys Run\Plugins\PasswordGenerator\
3. 重启 PowerToys

# 使用
Alt+Space → pw → Enter    # 生成 16 位密码
Alt+Space → pw 20 → Enter # 生成 20 位密码
Alt+Space → pw pin → Enter # 生成 6 位 PIN
```

## 项目状态

- **当前版本**: v1.0.0
- **开发状态**: 稳定可用
- **V2 计划**: 🔑 矢量图标、Diceware 密码短语模式、自定义排除字符

## 许可证

MIT License - 自由使用、修改、分发

---

**作者**: JoeSec | **GitHub**: [JoeSec/PowerToysPasswordGenerator](https://github.com/JoeSec/PowerToysPasswordGenerator)
