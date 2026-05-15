using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using ts3to6.Services;

namespace ts3to6.Components.Pages
{
    public partial class Home
    {
        [Inject]
        private TsDbService DbService { get; set; } = default!;
        [Inject]
        private MigrationService Migrator { get; set; } = default!;
        [Inject]
        private NavigationManager Nav { get; set; } = default!;
        [Inject]
        private DownloadCache Cache { get; set; } = default!;


        private bool _converting = false;
        public bool Converting
        {
            get => _converting;
            set
            {
                if (_converting != value)
                {
                    _converting = value;
                    StateHasChanged();
                }
            }
        }   

        private bool _dragOver = false;
        public bool DragOver
        {
            get => _dragOver;
            set
            {
                if (_dragOver != value)
                {
                    _dragOver = value;
                    StateHasChanged();
                }
            }
        }
        private int _dragCount = 0;
        public int DragCount
        {
            get => _dragCount;
            set
            {
                if (_dragCount != value)
                {
                    _dragCount = value;
                    StateHasChanged();
                }
            }
        }

        private MigrationService.MigrationResult? _result;

        private async Task OnFileSelected(InputFileChangeEventArgs e)
        {
            _dragOver = false;
            _dragCount = 0;

            using var memoryStream = new MemoryStream();
            await e.File.OpenReadStream(maxAllowedSize: 512 * 1024 * 1024).CopyToAsync(memoryStream);
            byte[] fileBytes = memoryStream.ToArray();

            Converting = true;
            bool loaded = await DbService.LoadAsync(fileBytes);

            if (!loaded)
            {
                // ErrorMessage is already set in TsDbService
                Converting = false;
                return;
            }

            _result = await Migrator.RunAsync();
            Converting = false;
        }

        private void Reset()
        {
            DbService.Reset();
            _result = null;
            _converting = false;
            StateHasChanged();
        }

        private void OnDragEnter() { _dragCount++; _dragOver = _dragCount > 0; }
        private void OnDragLeave() { _dragCount = Math.Max(0, _dragCount - 1); _dragOver = _dragCount > 0; }

        private string DownloadUrl(string id) => Nav.ToAbsoluteUri($"/dl/{id}/tsserver.sqlitedb").ToString();

        private static string FormatTtl(TimeSpan t) =>
        t.TotalHours >= 1 ? $"{t.TotalHours:F0}h" : $"{t.TotalMinutes:F0} min";

        private static string FormatSize(long bytes) => bytes switch
        {
            < 1_024 => $"{bytes} o",
            < 1_024 * 1_024 => $"{bytes / 1_024.0:F1} Ko",
            _ => $"{bytes / (1_024.0 * 1_024):F2} Mo",
        };
    }
}