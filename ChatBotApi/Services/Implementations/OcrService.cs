using ChatBotApi.Services.Interfaces;
using Tesseract;

namespace ChatBotApi.Services.Implementations
{
    public class OcrService : IOcrService
    {
        public string ExtrairTextoDeImagem(string caminhoImagem)
        {
            try
            {
                // Caminho para a pasta "tessdata"
                var tessDataPath = @"C:\Program Files\Tesseract-OCR\tessdata";

                // O segundo parâmetro é o idioma: "eng" ou "por"
                using var engine = new TesseractEngine(tessDataPath, "eng", EngineMode.Default);
                using var img = Pix.LoadFromFile(caminhoImagem);
                using var page = engine.Process(img);

                return page.GetText();
            }
            catch (Exception ex)
            {
                return $"Erro ao processar a imagem: {ex.Message}";
            }
        }
    }
}
