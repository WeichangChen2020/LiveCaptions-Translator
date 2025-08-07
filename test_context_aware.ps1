# 测试ContextAware功能是否正常工作

# 1. 确保应用程序已关闭
Write-Host "请确保LiveCaptions-Translator应用程序已关闭，然后按任意键继续..."
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")

# 2. 修改setting.json文件，启用ContextAware
$settingPath = "$env:APPDATA\LiveCaptions-Translator\setting.json"
if (Test-Path $settingPath) {
    Write-Host "正在修改setting.json文件..."
    $setting = Get-Content $settingPath -Raw | ConvertFrom-Json
    $setting.contextAware = $true
    $setting | ConvertTo-Json -Depth 10 | Set-Content $settingPath
    Write-Host "已成功启用ContextAware功能！"
} else {
    Write-Host "未找到setting.json文件，请先运行应用程序以生成配置文件。"
    exit
}

Write-Host "\n测试完成！请启动应用程序并测试对话记忆功能。"
Write-Host "提示：在应用程序中，确保'Context Aware'开关已打开。"