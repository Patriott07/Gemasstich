using System;
using DG.Tweening;
using UnityEngine;

public class CameraPanDrag : MonoBehaviour
{
    [Header("Pengaturan Geser Kamera")]
    public float panSpeed = 15f;
    public float smoothSpeed = 10f; // Kecepatan menyusul (Makin kecil = makin melayang/licin)
    private Vector3 dragOrigin;
    private Vector3 targetPosition; // Menyimpan titik tujuan akhir kamera

    [Header("Pengaturan Zoom Kamera")]
    public float zoomSpeed = 5f; // Aku besarkan sedikit agar lebih responsif untuk sistem target
    public float minZoom = 2f;
    public float maxZoom = 12f;
    public float zoomSmoothSpeed = 10f; // Efek empuk saat nge-zoom
    private float targetZoom; // Menyimpan ukuran tujuan akhir zoom
    public Transform targetSorot;
    private Camera cam;

    void Awake()
    {
        cam = GetComponent<Camera>();

        // Saat mulai, target posisi dan zoom disamakan dengan posisi asli agar tidak loncat
        targetPosition = transform.position;
        targetZoom = cam.orthographicSize;
    }

    void Update()
    {
        Zoom();
        RotateCam();
    }

    void RotateCam()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Vector3 newRotate = cam.transform.rotation.eulerAngles;
            newRotate.y += 90;
            // Bikin Sequence (Urutan Animasi)
            Sequence seq = DOTween.Sequence();

            // 1. Putar kamera selama 1.4 detik
            seq.Append(cam.transform.DOLocalRotate(newRotate, 1.4f).SetEase(Ease.OutBack));

            // 2. Setelah putar selesai, geser POSISI kamera selama 1 detik (Rotasi tetap aman!)
            if (targetSorot != null)
            {
                // Posisi akhir = Posisi target + jarak kamera
                Vector3 targetPos = targetSorot.position;
                targetPosition = targetPos;
                // seq.Append(cam.transform.DOMove(targetPos, 1f).SetEase(Ease.InOutQuad));
            }
        }
    }

    void LateUpdate()
    {
        DragCam();

        // --- RAHASIA "GAME FEEL" PREMIUM ADA DI DUA BARIS INI ---

        // 1. Secara perlahan (smooth) menggerakkan posisi kamera menuju targetPosition
        transform.position = Vector3.Lerp(transform.position, targetPosition, smoothSpeed * Time.deltaTime);

        // 2. Secara perlahan mengubah ukuran layar menuju targetZoom
        cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetZoom, zoomSmoothSpeed * Time.deltaTime);
    }

    void Zoom()
    {
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");

        if (scrollInput != 0f)
        {
            // ALIH-ALIH MENGUBAH KAMERA LANGSUNG, KITA UBAH TARGETNYA
            targetZoom -= (scrollInput * zoomSpeed);
            targetZoom = Mathf.Clamp(targetZoom, minZoom, maxZoom);
        }
    }

    void DragCam()
    {
        if (Input.GetMouseButtonDown(0))
        {
            dragOrigin = Input.mousePosition;
            return;
        }

        if (!Input.GetMouseButton(0)) return;

        Vector3 pos = cam.ScreenToViewportPoint(Input.mousePosition - dragOrigin);
        Vector3 move = new Vector3(-pos.x * panSpeed, 0, -pos.y * panSpeed);
        move = Quaternion.Euler(0, transform.eulerAngles.y, 0) * move;

        // ALIH-ALIH MENGGERAKKAN KAMERA LANGSUNG, KITA TAMBAHKAN PERGERAKAN KE TARGET
        targetPosition += move;

        dragOrigin = Input.mousePosition;
    }
}
