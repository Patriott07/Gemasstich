using UnityEngine;
using System.Collections;
using DG.Tweening;
using UnityEditor;
public class WaterNode : MonoBehaviour
{
    [Header("Pengaturan Simulasi")]
    public GameObject waterPrefab;    // Masukkan prefab blok air biru ini sendiri ke sini
    public float delaySpread = 0.005f;  // Jeda waktu sebelum air menyebar (biar efeknya ngalir pelan)
    public float gridSize = 1f;       // Jarak antar blok (biasanya 1 di Unity)

    [Header("Efek Penghijauan")]
    public Material materialTanahHijau; // Masukkan material warna hijau di sini
    public float radiusPenghijauan = 2.1f; // Radius 2 blok (dikasih lebih 0.1f untuk toleransi)

    void Start()
    {
        // 1. Animasi muncul airnya pakai DOTween biar satisfying
        transform.localScale = Vector3.zero;
        transform.DOScale(0.5f, 0.04f).SetEase(Ease.OutBack).OnComplete(() =>
        {
            // Setelah animasi muncul selesai, mulai proses mikir (menyebar & menghijaukan)
            StartCoroutine(ProsesAir());
        });

        InvokeRepeating("SpreadWater", 0f, 1f);
    }

    IEnumerator ProsesAir()
    {
        // Tunggu sebentar biar pemain bisa lihat airnya ngalir secara visual
        yield return new WaitForSeconds(delaySpread);

        SpreadWater();
        GreeningEffect();
    }

    void SpreadWater()
    {
        // Mengecek ke 4 arah mata angin (Sumbu X dan Z)
        Vector3[] arahCek = { Vector3.forward, Vector3.back, Vector3.left, Vector3.right };
        foreach (Vector3 arah in arahCek)
        {
            // Menentukan titik kordinat kotak di sebelahnya
            Vector3 titikCek = transform.position + (arah * gridSize);

            // Bikin radar kecil (radius 0.2f) persis di titik kotak sebelah
            Collider[] hits = Physics.OverlapSphere(titikCek, 0.2f);

            foreach (Collider hit in hits)
            {
                // Kalau radar mendeteksi ada blok parit kosong
                if (hit.CompareTag("JalurAir"))
                {
                    // Ganti tag-nya biar nggak dipanggil dua kali sama blok air lain
                    hit.tag = "Untagged";

                    // Hancurkan blok parit kosong tersebut
                    Destroy(hit.gameObject);

                    // Lahirkan blok air baru di posisi tersebut! (Reaksi berantai terjadi)
                    Vector3 pos = new Vector3(titikCek.x, hit.transform.position.y - 0.2f, titikCek.z);
                    Instantiate(waterPrefab, pos, Quaternion.identity);
                }
            }
        }
    }

    void GreeningEffect()
    {
        // Bikin radar besar (radius 2 blok) di sekitar blok air ini
        Collider[] hits = Physics.OverlapSphere(transform.position, radiusPenghijauan);

        foreach (Collider hit in hits)
        {
            // Kalau radarnya kena Tanah Kering
            if (hit.CompareTag("TanahKering"))
            {
                // 1. Ganti tag-nya jadi Tanah Hijau (biar nggak dihijaukan berkali-kali)
                hit.tag = "TanahHijau";

                // 2. Ganti visualnya (warnanya) jadi hijau
                Renderer rend = hit.GetComponent<Renderer>();
                if (rend != null && materialTanahHijau != null)
                {
                    rend.material = materialTanahHijau;

                    // 3. Kasih efek DOTween membal (Punch) ke tanahnya biar kerasa kalau ada tanaman tumbuh!

                    // 1. Simpan posisi dan skala asli sebagai target akhir
                    Vector3 originalPosition = hit.transform.localPosition;
                    // originalPosition = originalPosition + (Vector3.up * 0.1f);
                    Vector3 originalScale = Vector3.one * 0.5f;

                    // 2. Set kondisi awal (sebelum animasi jalan):
                    // Turunkan posisinya ke bawah (-1.5f) dan jadikan skalanya 0 (hilang)
                    hit.transform.localPosition = new Vector3(originalPosition.x, originalPosition.y - 1.5f, originalPosition.z);
                    hit.transform.localScale = Vector3.zero;

                    Sequence greenSeq = DOTween.Sequence();

                    // 4. Animasi membesar ke ukuran asli dan naik ke posisi asli
                    // Ease.OutQuad bikin pergerakannya cepat di awal lalu melambat halus di akhir (tanpa membal)
                    float durasiAnimasi = 0.6f; // Silakan sesuaikan kalau mau lebih cepat/lambat

                    greenSeq.Join(hit.transform.DOScale(originalScale, durasiAnimasi).SetEase(Ease.OutQuad));
                    greenSeq.Join(hit.transform.DOLocalMoveY(originalPosition.y + 0.2f, durasiAnimasi).SetEase(Ease.OutQuad));
                    // greenSeq.Join(hit.transform.DOPunchScale(Vector3.one * 0.2f, durasiAnimasi, 5).SetEase(Ease.InBack));
                    greenSeq.SetDelay(0.1f);
                    // 5. (Opsional tapi direkomendasikan) Pastikan ukurannya presisi di akhir animasi
                    greenSeq.OnComplete(() =>
                    {
                        hit.transform.localScale = originalScale;
                        hit.transform.DOLocalMoveY(originalPosition.y, 0.1f);
                        // hit.transform.localPosition = originalPosition;
                    });

                }
            }
        }

    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radiusPenghijauan);
    }
}
