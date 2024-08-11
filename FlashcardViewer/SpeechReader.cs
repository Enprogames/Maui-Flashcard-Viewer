using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;

namespace FlashcardViewer
{
    public class SpeechReader
    {
        private readonly ILogger<SpeechReader> logger;
        private CancellationTokenSource cts;

        public SpeechReader(ILogger<SpeechReader> logger)
        {
            this.logger = logger;
            cts = new CancellationTokenSource();
        }

        public async Task SpeakAsync(string text)
        {
            CancelSpeech();

            try
            {
                logger.LogInformation("Starting to speak: {text}", text);
                await TextToSpeech.SpeakAsync(text, cancelToken: cts.Token);
                logger.LogInformation("Finished speaking: {text}", text);
            }
            catch (OperationCanceledException)
            {
                logger.LogInformation("Speech was canceled.");
            }
        }

        public void CancelSpeech()
        {
            if (!(cts?.IsCancellationRequested ?? true))
            {
                logger.LogInformation("Cancelling speech.");
                cts.Cancel();
                cts = new CancellationTokenSource();
            }
        }
    }
}
