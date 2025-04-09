using Smartplug.Domain.Enums;

namespace Smartplug.Domain.Entities
{
   public class Schedule
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid DeviceId { get; set; }
    public Device Device { get; set; }

    /// <summary>
    /// Zamanlamanın tipi: OneTime (tek seferlik) veya Recurring (tekrarlayan)
    /// </summary>
    public ScheduleType Type { get; set; }

    // Tek seferlik zamanlamalar için:
    public DateTime? ScheduledTime { get; set; }  // İşlem yapılacak kesin tarih-saat
    public bool? DesiredStatus { get; set; }        // true = aç, false = kapat
    public bool? Executed { get; set; }             // İşlem gerçekleşti mi?

    // Tekrarlayan zamanlamalar için:
    public DayOfWeek? RecurringDay { get; set; }       // Örneğin Pazartesi
    public TimeSpan? StartTimeOfDay { get; set; }        // Cihazın açılacağı saat (örn: 09:00)
    public TimeSpan? EndTimeOfDay { get; set; }          // Cihazın kapatılacağı saat (örn: 18:00)

    /// <summary>
    /// Oluşturulan Hangfire job ID’leri (tek seferlik veya tekrarlayan aç/kapat job’ları için)
    /// </summary>
    public string? HangfireJobId { get; set; }        // Tek seferlik işlemde
    public string? HangfireJobIdOn { get; set; }        // Tekrarlayan işlemde cihaz açma job’ı
    public string? HangfireJobIdOff { get; set; }       // Tekrarlayan işlemde cihaz kapatma job’ı

    // Genel aktiflik durumu – recuring için aktif/pasif bilgisini temsil edebilir.
    public bool IsEnabled { get; set; } = true;
}

}
