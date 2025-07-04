# CustomLightBrowser – MSIX Packaged Minimal WebView2 App

This is a lightweight Windows desktop browser app built using WPF (.NET 8) and WebView2. It's wrapped as a Microsoft Store-compatible MSIX package for easy deployment and sideloading.

## 🔧 Features

- ✅ Clean WPF UI with WebView2 integration
- ✅ MSIX / .appxbundle packaging with digital certificate support
- ✅ Supports trusted/silent installations
- ✅ Architecture-specific build targeting x64
- ✅ Visual Studio 2022 build with .NET 8

## 📦 Tech Stack

- .NET 8
- WPF
- WebView2
- MSIX Packaging Tool
- Visual Studio 2022

## 📁 Project Structure

- `CustomLightBrowser/`: WPF application project
- `CustomLightBrowser.Packaging/`: MSIX packaging project

## 🛠️ Build Instructions

1. Open the solution in Visual Studio 2022
2. Restore NuGet packages
3. Set `CustomLightBrowser.Packaging` as the startup project
4. Select `x64` + `Release` from Configuration Manager
5. Build or use **Publish > Create App Packages** to generate `.appxbundle`

## 📦 Output

- `CustomLightBrowser.Packaging\AppPackages\...`: Contains the `.appxbundle` for sideloading

---

## 📄 License

This project is licensed under the [MIT License](LICENSE).  
You are free to use, modify, and distribute it for personal or commercial use.

---

## 🙌 Author

Made with 💙 by [@harmander-singh](https://github.com/harmander-singh)  
Contributions, stars, and feedback are welcome!
