using UnityEngine;

public class ShowColliderGizmo : MonoBehaviour
{
    //ก็อปมาโค้ดนี้ เอาไว้ดู Collider

    // กำหนดสีของเส้น Gizmo ที่จะแสดง
    [SerializeField] private Color gizmoColor = Color.green;

    // ตัวแปรสำหรับเก็บ Collider (จะหาเองตอน Start)
    private Collider2D myCollider;

    void Start()
    {
        // พยายามหา Collider2D ที่ติดอยู่กับ GameObject นี้
        myCollider = GetComponent<Collider2D>();
    }

    // ฟังก์ชันนี้จะถูกเรียกใน Scene View ตลอดเวลา (แม้เกมจะไม่ได้เล่น)
    void OnDrawGizmos()
    {
        // ถ้ายังไม่ได้หา Collider หรือหาไม่เจอ ก็พยายามหาอีกครั้ง
        if (myCollider == null)
        {
            myCollider = GetComponent<Collider2D>();
        }

        // ถ้าหา Collider เจอแล้ว และมันเป็น PolygonCollider2D
        if (myCollider != null && myCollider is PolygonCollider2D)
        {
            // กำหนดสีของ Gizmo
            Gizmos.color = gizmoColor;

            // วาดเส้น Polygon Collider2D
            PolygonCollider2D polyCollider = (PolygonCollider2D)myCollider;
            for (int i = 0; i < polyCollider.points.Length; i++)
            {
                Vector2 p1 = transform.TransformPoint(polyCollider.points[i]);
                Vector2 p2 = transform.TransformPoint(polyCollider.points[(i + 1) % polyCollider.points.Length]);
                Gizmos.DrawLine(p1, p2);
            }
        }
        // ถ้าเป็น BoxCollider2D (เผื่ออนาคต)
        else if (myCollider != null && myCollider is BoxCollider2D)
        {
            Gizmos.color = gizmoColor;
            Gizmos.DrawWireCube(myCollider.bounds.center, myCollider.bounds.size);
        }
    }
}
