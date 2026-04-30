using UnityEngine;
using DG.Tweening;
public class CameraInteraction : MonoBehaviour
{
    public GameObject WaterPrefab;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }
    void SpawnWater()
    {
        // Membuat laser dari posisi mouse di layar ke dunia 3D
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        // Jika laser mengenai sesuatu (lantai/objek)
        if (Physics.Raycast(ray, out hit))
        {
            // Suruh agent pergi ke titik sentuhan laser tersebut
            if (hit.transform.CompareTag("JalurAir"))
            {
                Debug.Log(hit.transform.gameObject.name);

                Transform tObj = hit.transform;
                Destroy(hit.transform.gameObject);
                
                Vector3 pos = new Vector3(tObj.position.x, tObj.position.y - 0.2f, tObj.position.z);
                Instantiate(WaterPrefab, pos, tObj.rotation);

            }
        }
    }

    void SetHole()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        // Jika laser mengenai sesuatu (lantai/objek)
        if (Physics.Raycast(ray, out hit))
        {
            // Suruh agent pergi ke titik sentuhan laser tersebut
            if (hit.transform.CompareTag("AvaibleSkop") || hit.transform.CompareTag("TanahKering") || hit.transform.CompareTag("TanahHijau") )
            {
                Transform tObj = hit.transform;
                tObj.gameObject.tag = "JalurAir";
                
                Debug.Log(hit.transform.gameObject.name);

                // saat animasinya masih berjalan
                hit.collider.enabled = false;

                // 2. Simpan posisi dan skala aslinya buat dibalikin nanti
                Vector3 originalScale = tObj.localScale;
                Vector3 originalPosition = tObj.localPosition;

                // 3. Bikin Sequence DOTween biar animasinya jalan barengan
                Sequence digSeq = DOTween.Sequence();

                // Animasi Mengecil & Turun ke bawah (pakai Ease.InBack biar ada efek "terisap" / membal ke dalam)
                digSeq.Join(tObj.DOScale(Vector3.zero, 0.3f).SetEase(Ease.InBack));
                digSeq.Join(tObj.DOLocalMoveY(originalPosition.y - 1.5f, 0.3f).SetEase(Ease.InBack));
                
                // 4. Apa yang terjadi setelah bloknya masuk ke tanah?
                digSeq.OnComplete(() =>
                {
                    // Matikan visualnya (jadi bolong/parit)
                    tObj.gameObject.GetComponent<MeshRenderer>().enabled = false;
                    

                    // PRO TRICK: Dibalikin lagi ke posisi semula secara diam-diam!
                    // Karena Mesh-nya udah mati, pemain gak akan lihat ini terjadi.
                    // Jadi kalau nanti mau dimunculin lagi (tinggal nyalain MeshRenderer), dia udah ready di tempat aslinya!
                    tObj.localScale = originalScale;
                    tObj.localPosition = originalPosition;

                    // Jangan lupa nyalain lagi collider-nya (opsional, tergantung kamu butuh 
                    // lubangnya bisa diklik lagi atau nggak)
                    hit.collider.enabled = true;

                    // (Di sinilah kamu naruh script spawn/ubah tag jadi JalurAir kalau butuh)
                });
            }
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F)) SpawnWater();
        if (Input.GetKeyDown(KeyCode.E)) SetHole();
    }
}
