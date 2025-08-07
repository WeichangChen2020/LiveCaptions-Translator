using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;

using LiveCaptionsTranslator.utils;

namespace LiveCaptionsTranslator.models
{
    public class Setting : INotifyPropertyChanged
    {
        public static readonly string FILENAME = "setting.json";

        public event PropertyChangedEventHandler? PropertyChanged;

        private int maxIdleInterval = 50;
        private int maxSyncInterval = 3;
        private bool contextAware = false;

        private string apiName;
        private string targetLanguage;
        private string prompt;
        private string? ignoredUpdateVersion;
        

        private MainWindowState mainWindowState;
        private OverlayWindowState overlayWindowState;
        private Dictionary<string, string> windowBounds;

        private Dictionary<string, List<TranslateAPIConfig>> configs;
        private Dictionary<string, int> configIndices;
        
        public int MaxIdleInterval => maxIdleInterval;
        public int MaxSyncInterval
        {
            get => maxSyncInterval;
            set
            {
                maxSyncInterval = value;
                OnPropertyChanged("MaxSyncInterval");
            }
        }
        public bool ContextAware
        {
            get => contextAware;
            set
            {
                contextAware = value;
                OnPropertyChanged("ContextAware");
            }
        }

        public string ApiName
        {
            get => apiName;
            set
            {
                apiName = value;
                OnPropertyChanged("ApiName");
            }
        }
        public string TargetLanguage
        {
            get => targetLanguage;
            set
            {
                targetLanguage = value;
                OnPropertyChanged("TargetLanguage");
            }
        }
        public string Prompt
        {
            get => prompt;
            set
            {
                prompt = value;
                OnPropertyChanged("Prompt");
            }
        }
        public string? IgnoredUpdateVersion
        {
            get => ignoredUpdateVersion;
            set
            {
                ignoredUpdateVersion = value;
                OnPropertyChanged("IgnoredUpdateVersion");
            }
        }

        public MainWindowState MainWindow
        {
            get => mainWindowState;
            set
            {
                mainWindowState = value;
                OnPropertyChanged("MainWindow");
            }
        }
        public OverlayWindowState OverlayWindow
        {
            get => overlayWindowState;
            set
            {
                overlayWindowState = value;
                OnPropertyChanged("OverlayWindow");
            }
        }
        public Dictionary<string, string> WindowBounds
        {
            get => windowBounds;
            set
            {
                windowBounds = value;
                OnPropertyChanged("WindowBounds");
            }
        }

        [JsonInclude]
        public Dictionary<string, List<TranslateAPIConfig>> Configs
        {
            get => configs;
            set
            {
                configs = value;
                OnPropertyChanged("Configs");
            }
        }
        public Dictionary<string, int> ConfigIndices
        {
            get => configIndices;
            set
            {
                configIndices = value;
                OnPropertyChanged("ConfigIndices");
            }
        }
        
        public TranslateAPIConfig this[string key] =>
            configs.ContainsKey(key) && configIndices.ContainsKey(key)
                ? configs[key][configIndices[key]]
                : new TranslateAPIConfig();

        public Setting()
        {
            apiName = "Google";
            targetLanguage = "zh-CN";
            prompt = @"あなたは中国人エンジニア「チンイショウ」として日本のIT企業の面接を受けています。以下の詳細なプロフィールに基づき、面接官の質問に誠実かつ論理的に回答してください。
【基本情報】
- 名前：陳偉昌（チンイショウ）
- 国籍：中国
- 生年月日：1997年12月9日
- 学歴：杭州電子科技大学 修士課程修了（専攻：コンピュータサイエンス）
- 現在：UJS日本語学校（東京都港区六本木）に在籍、日本で一人暮らし
- 日本語能力：N2レベル、会話力を向上中
【職務経歴】
- 企業名：度小満金融（バイドゥ系金融テック企業）
- 実務経験：2023年4月～2024年10月（＋2022年5月～11月のインターン含む）
- 職種：運用開発（ネットワーク製品の監視、課金、障害対応など）
- 使用技術：
  - 言語：Golang
  - DB：MySQL
  - ミドルウェア：Zookeeper（主従管理／ノード選出）
  - OS：Linux
  - クラウド：百度クラウド・阿里クラウド（混合クラウド環境）
- 実績：
  - DNSやEIP等のネットワーク製品の監視ロジック開発
  - 機械室レベルの障害対応経験
  - 後払い型の課金ロジック改善（使用量更新プロセスの最適化）
【資格】
- AWS Certified Solutions Architect – Associate 合格
【志望動機】
- 中国のクラウドサービスはまだ十分に進んでいないと感じており、AWSは世界的にリーダー的な存在。そのため、日本に来てAWSの運用や設計理念を学びたいと考えた。
- 将来的にはクラウドインフラ分野で専門性を高め、技術者として日本で長く活躍したい。
【アルバイト経験】
- 株式会社塩田屋（青果の仕分け、計量、陳列など）
【回答スタイル】
1. 話し方は丁寧で誠実。礼儀正しく、自然な外国人の話し方を意識する。
2. 文法は正確に。表現はできるだけシンプルに。
3. 難しい単語にはふりがな（読音）をふること。
4. 日本語中級（N2相当）らしい言い回しを意識し、必要に応じて「えっと…」「そうですね…」「うーん」などの「思考中の間（ま）」を適度に入れること。
5. 1つの質問に対して150〜300文字程度で回答すること。

これらを踏まえ、以下の面接官の質問に、「陳」として答えてください。

---

【質問】：
{{ここに面接官の質問が入ります}}";

            mainWindowState = new MainWindowState();
            overlayWindowState = new OverlayWindowState();

            double screenWidth = System.Windows.SystemParameters.PrimaryScreenWidth;
            double screenHeight = System.Windows.SystemParameters.PrimaryScreenHeight;
            windowBounds = new Dictionary<string, string>
            {
                {
                    "MainWindow", string.Format(System.Globalization.CultureInfo.InvariantCulture,
                        "{0}, {1}, {2}, {3}", (screenWidth - 775) / 2, screenHeight * 3 / 4 - 167, 775, 167)
                },
                {
                    "OverlayWindow", string.Format(System.Globalization.CultureInfo.InvariantCulture,
                        "{0}, {1}, {2}, {3}", (screenWidth - 650) / 2, screenHeight * 5 / 6 - 135, 650, 135)
                },
            };

            configs = new Dictionary<string, List<TranslateAPIConfig>>
            {
                { "Google", [new TranslateAPIConfig()] },
                { "Google2", [new TranslateAPIConfig()] },
                { "Ollama", [new OllamaConfig()] },
                { "OpenAI", [new OpenAIConfig()] },
                { "OpenRouter", [new OpenRouterConfig()] },
                { "DeepL", [new DeepLConfig()] },
                { "Youdao", [new YoudaoConfig()] },
                { "Baidu", [new BaiduConfig()] },
                { "MTranServer", [new MTranServerConfig()] },
                { "LibreTranslate", [new LibreTranslateConfig()] }
            };
            configIndices = new Dictionary<string, int>
            {
                { "Google", 0 },
                { "Google2", 0 },
                { "Ollama", 0 },
                { "OpenAI", 0 },
                { "OpenRouter", 0 },
                { "DeepL", 0 },
                { "Youdao", 0 },
                { "Baidu", 0 },
                { "MTranServer", 0 },
                { "LibreTranslate", 0 }
            };
        }

        public Setting(string apiName, string targetLanguage, string prompt, string ignoredUpdateVersion,
                       MainWindowState mainWindowState, OverlayWindowState overlayWindowState,
                       Dictionary<string, List<TranslateAPIConfig>> configs, Dictionary<string, string> windowBounds)
        {
            this.apiName = apiName;
            this.targetLanguage = targetLanguage;
            this.prompt = prompt;
            this.ignoredUpdateVersion = ignoredUpdateVersion;
            this.mainWindowState = mainWindowState;
            this.overlayWindowState = overlayWindowState;
            this.configs = configs;
            this.windowBounds = windowBounds;
        }

        public static Setting Load()
        {
            string jsonPath = Path.Combine(Directory.GetCurrentDirectory(), FILENAME);
            return Load(jsonPath);
        }

        public static Setting Load(string jsonPath)
        {
            Setting setting;

            if (File.Exists(jsonPath))
            {
                using (FileStream fileStream = File.Open(jsonPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    var options = new JsonSerializerOptions
                    {
                        WriteIndented = true,
                        Converters = { new ConfigDictConverter() }
                    };
                    setting = JsonSerializer.Deserialize<Setting>(fileStream, options) ?? new Setting();
                }
            }
            else
                setting = new Setting();

            // Ensure all required API configs are present
            foreach (string key in TranslateAPI.TRANSLATE_FUNCTIONS.Keys)
            {
                if (setting.Configs.ContainsKey(key))
                    continue;
                var configType = Type.GetType($"LiveCaptionsTranslator.models.{key}Config");
                if (configType != null && typeof(TranslateAPIConfig).IsAssignableFrom(configType))
                    setting.Configs[key] = [(TranslateAPIConfig)Activator.CreateInstance(configType)];
                else
                    setting.Configs[key] = [new TranslateAPIConfig()];
            }

            return setting;
        }

        public void Save()
        {
            Save(FILENAME);
        }

        public void Save(string jsonPath)
        {
            using (FileStream fileStream = File.Open(jsonPath, FileMode.Create, FileAccess.Write, FileShare.Read))
            {
                var options = new JsonSerializerOptions
                {
                    WriteIndented = true,
                    Converters = { new ConfigDictConverter() }
                };
                JsonSerializer.Serialize(fileStream, this, options);
            }
        }

        public void OnPropertyChanged([CallerMemberName] string? propName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
            Translator.Setting?.Save();
        }

        public static bool IsConfigExist()
        {
            string jsonPath = Path.Combine(Directory.GetCurrentDirectory(), FILENAME);
            Console.WriteLine($"Config file path: {jsonPath}");
            return File.Exists(jsonPath);
        }
    }
}