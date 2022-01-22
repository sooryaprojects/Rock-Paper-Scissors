using System.Collections;
using System.Collections.Generic;
using RuntimeDebugDraw;
using UnityEngine;
public class DrawX : MonoBehaviour {

    // public Material mat;
    public Material mat;
    public static bool shouldDraw = false;
    //TODO 
    //fix color setting
    //it doesn't work correctly 
    //because it's a single material 
    //that gets overriden by the end of the frame.
    //Use GL color? or material color

    //TODO
    //add rotation support
    //https://math.stackexchange.com/questions/270194/how-to-find-the-vertices-angle-after-rotation
    //NIGHTMARE, BOI

    //TODO
    //add support for quad instead of just wireframes

    void Awake () {
        // mat = new Material(Shader.Find("Custom/DebugShader"));
    }
    //renders shapes
    void DrawShapes () {
        //cube
        for (int i = 0; i < cubes.Count; i++) {
            mat.SetPass (0);
            mat.SetColor ("_Color", cubes[i].color);
            cubes[i].Render ();
        }

        //sphere
        for (int i = 0; i < spheres.Count; i++) {

            mat.SetPass (0);
            mat.SetColor ("_Color", spheres[i].color);
            spheres[i].Render ();
        }

        //line
        for (int i = 0; i < lines.Count; i++) {
            mat.SetPass (0);
            mat.SetColor ("_Color", lines[i].color);
            lines[i].Render ();
        }

    }

    static List<Cube> cubes = new List<Cube> ();
    static List<Sphere> spheres = new List<Sphere> ();
    static List<Line> lines = new List<Line> ();

    //creates and adds cubes to the list of cubes to be rendered
    public static void Cube (Vector3 pos, float size, Color col = default (Color)) {
        if (!shouldDraw)
            return;

        Vector3[] cubeVerts = new Vector3[8];
        cubeVerts[0] = new Vector3 (pos.x - size / 2, pos.y + size / 2, pos.z + size / 2);
        cubeVerts[1] = new Vector3 (pos.x + size / 2, pos.y + size / 2, pos.z + size / 2);

        cubeVerts[2] = new Vector3 (pos.x - size / 2, pos.y + size / 2, pos.z - size / 2);
        cubeVerts[3] = new Vector3 (pos.x + size / 2, pos.y + size / 2, pos.z - size / 2);

        cubeVerts[4] = new Vector3 (pos.x - size / 2, pos.y - size / 2, pos.z + size / 2);
        cubeVerts[5] = new Vector3 (pos.x + size / 2, pos.y - size / 2, pos.z + size / 2);

        cubeVerts[6] = new Vector3 (pos.x - size / 2, pos.y - size / 2, pos.z - size / 2);
        cubeVerts[7] = new Vector3 (pos.x + size / 2, pos.y - size / 2, pos.z - size / 2);

        cubes.Add (new Cube (cubeVerts, col));
    }

    public static void CubeRot (Vector3 pos, Vector3 rot, float size, Color col = default (Color)) {
        if (!shouldDraw)
            return;
        Vector3[] cubeVerts = new Vector3[8];
        cubeVerts[0] = new Vector3 (pos.x - size / 2, pos.y + size / 2, pos.z + size / 2);
        cubeVerts[1] = new Vector3 (pos.x + size / 2, pos.y + size / 2, pos.z + size / 2);

        cubeVerts[2] = new Vector3 (pos.x - size / 2, pos.y + size / 2, pos.z - size / 2);
        cubeVerts[3] = new Vector3 (pos.x + size / 2, pos.y + size / 2, pos.z - size / 2);

        cubeVerts[4] = new Vector3 (pos.x - size / 2, pos.y - size / 2, pos.z + size / 2);
        cubeVerts[5] = new Vector3 (pos.x + size / 2, pos.y - size / 2, pos.z + size / 2);

        cubeVerts[6] = new Vector3 (pos.x - size / 2, pos.y - size / 2, pos.z - size / 2);
        cubeVerts[7] = new Vector3 (pos.x + size / 2, pos.y - size / 2, pos.z - size / 2);

        for (int i = 0; i < cubeVerts.Length; i++) {
            Quaternion rotation = Quaternion.Euler (rot);
            Matrix4x4 m = Matrix4x4.Rotate (rotation);
            cubeVerts[i] = m.MultiplyPoint3x4 (cubeVerts[i]);
        }
        cubes.Add (new Cube (cubeVerts, col));
    }

    //creates and adds spheres to the list of spheres to be rendered
    public static void Sphere (Vector3 pos, float radius, int resolution = 10, Color col = default (Color)) {
        if (!shouldDraw)
            return;
        col = Color.red;
        float angle = 0;
        Vector3[] sphereVertsY = new Vector3[resolution + 1];
        Vector3[] sphereVertsX = new Vector3[resolution + 1];
        Vector3[] sphereVertsZ = new Vector3[resolution + 1];

        for (int i = 0; i < (resolution + 1); i++) {
            sphereVertsY[i] = new Vector3 (pos.x + Mathf.Sin (Mathf.Deg2Rad * angle) * radius,
                pos.y,
                pos.z + Mathf.Cos (Mathf.Deg2Rad * angle) * radius);

            angle += (360f / resolution);
        }

        for (int i = 0; i < (resolution + 1); i++) {
            sphereVertsX[i] = new Vector3 (pos.x,
                pos.y + Mathf.Cos (Mathf.Deg2Rad * angle) * radius,
                pos.z + Mathf.Sin (Mathf.Deg2Rad * angle) * radius);

            angle += (360f / resolution);
        }

        for (int i = 0; i < (resolution + 1); i++) {
            sphereVertsZ[i] = new Vector3 (pos.x + Mathf.Sin (Mathf.Deg2Rad * angle) * radius,
                pos.y + Mathf.Cos (Mathf.Deg2Rad * angle) * radius,
                pos.z);

            angle += (360f / resolution);
        }

        spheres.Add (new Sphere (sphereVertsX, col));
        spheres.Add (new Sphere (sphereVertsY, col));
        spheres.Add (new Sphere (sphereVertsZ, col));
    }

    //creates and adds spheres to the list of spheres to be rendered
    public static void SphereRot (Vector3 pos, float radius, int resolution, Vector3 rot, Color col = default (Color)) {
        if (!shouldDraw)
            return;
        float angle = 0;
        Vector3[] sphereVertsY = new Vector3[resolution + 1];
        Vector3[] sphereVertsX = new Vector3[resolution + 1];
        Vector3[] sphereVertsZ = new Vector3[resolution + 1];

        for (int i = 0; i < (resolution + 1); i++) {
            sphereVertsY[i] = new Vector3 (pos.x + Mathf.Sin (Mathf.Deg2Rad * angle) * radius,
                pos.y,
                pos.z + Mathf.Cos (Mathf.Deg2Rad * angle) * radius);

            angle += (360f / resolution);

            Quaternion rotation = Quaternion.Euler (rot);
            Matrix4x4 m = Matrix4x4.Rotate (rotation);
            sphereVertsY[i] = m.MultiplyPoint3x4 (sphereVertsY[i]);

        }

        for (int i = 0; i < (resolution + 1); i++) {
            sphereVertsX[i] = new Vector3 (pos.x,
                pos.y + Mathf.Cos (Mathf.Deg2Rad * angle) * radius,
                pos.z + Mathf.Sin (Mathf.Deg2Rad * angle) * radius);

            angle += (360f / resolution);
            Quaternion rotation = Quaternion.Euler (rot);
            Matrix4x4 m = Matrix4x4.Rotate (rotation);
            sphereVertsX[i] = m.MultiplyPoint3x4 (sphereVertsX[i]);
        }

        for (int i = 0; i < (resolution + 1); i++) {
            sphereVertsZ[i] = new Vector3 (pos.x + Mathf.Sin (Mathf.Deg2Rad * angle) * radius,
                pos.y + Mathf.Cos (Mathf.Deg2Rad * angle) * radius,
                pos.z);

            angle += (360f / resolution);
            Quaternion rotation = Quaternion.Euler (rot);
            Matrix4x4 m = Matrix4x4.Rotate (rotation);
            sphereVertsZ[i] = m.MultiplyPoint3x4 (sphereVertsZ[i]);
        }

        spheres.Add (new Sphere (sphereVertsX, col));
        spheres.Add (new Sphere (sphereVertsY, col));
        spheres.Add (new Sphere (sphereVertsZ, col));
    }

    //creates and adds lines to the list of lines to be rendered
    public static void Line (Vector3 point1, Vector3 point2, Color col = default (Color)) {
        if (!shouldDraw)
            return;
        lines.Add (new Line (new Vector3[] { point1, point2 }, col));
    }

    //clear all GL lines for all shapes every frame
    void ClearShapes () {
        cubes.Clear ();
        spheres.Clear ();
        lines.Clear ();
    }

    //draw and clear
    void OnPostRender () {
        if (!shouldDraw)
            return;
        DrawShapes ();
        ClearShapes ();
    }
}

public class Shape {
    public Color color;
    public Vector3[] verts;
    public Shape (Vector3[] verts, Color color) {
        this.verts = verts;
        this.color = color;
    }
}

public class Cube : Shape {
    public Cube (Vector3[] verts, Color color) : base (verts, color) {

    }

    public void Render () {
        GL.Begin (GL.LINES);
        RuntimeDebugDraw.Draw.DrawLine (verts[0], verts[1]);
        RuntimeDebugDraw.Draw.DrawLine (verts[2], verts[3]);
        RuntimeDebugDraw.Draw.DrawLine (verts[4], verts[5]);
        RuntimeDebugDraw.Draw.DrawLine (verts[6], verts[7]);

        RuntimeDebugDraw.Draw.DrawLine (verts[0], verts[2]);
        RuntimeDebugDraw.Draw.DrawLine (verts[1], verts[3]);
        RuntimeDebugDraw.Draw.DrawLine (verts[4], verts[6]);
        RuntimeDebugDraw.Draw.DrawLine (verts[5], verts[7]);

        RuntimeDebugDraw.Draw.DrawLine (verts[0], verts[4]);
        RuntimeDebugDraw.Draw.DrawLine (verts[1], verts[5]);
        RuntimeDebugDraw.Draw.DrawLine (verts[2], verts[6]);
        RuntimeDebugDraw.Draw.DrawLine (verts[3], verts[7]);

        // GL.Vertex(verts[0]);
        // GL.Vertex(verts[1]);
        // GL.Vertex(verts[2]);
        // GL.Vertex(verts[3]);
        // GL.Vertex(verts[4]);
        // GL.Vertex(verts[5]);
        // GL.Vertex(verts[6]);
        // GL.Vertex(verts[7]);

        // GL.Vertex(verts[0]);
        // GL.Vertex(verts[2]);
        // GL.Vertex(verts[1]);
        // GL.Vertex(verts[3]);
        // GL.Vertex(verts[4]);
        // GL.Vertex(verts[6]);
        // GL.Vertex(verts[5]);
        // GL.Vertex(verts[7]);

        // GL.Vertex(verts[0]);
        // GL.Vertex(verts[4]);
        // GL.Vertex(verts[1]);
        // GL.Vertex(verts[5]);
        // GL.Vertex(verts[2]);
        // GL.Vertex(verts[6]);
        // GL.Vertex(verts[3]);
        // GL.Vertex(verts[7]);

        GL.End ();
    }
}

public class Sphere : Shape {
    public Sphere (Vector3[] verts, Color color) : base (verts, color) {

    }

    public void Render () {
        GL.Begin (GL.LINES);

        for (int j = 0; j < verts.Length; j++) {
            // GL.Vertex(verts[j]);
            if (j < verts.Length - 1) {
                RuntimeDebugDraw.Draw.DrawLine (verts[j], verts[j + 1], Color.red);

                // GL.Vertex(verts[j + 1]);
            }
        }

        GL.End ();
    }
}

public class Line : Shape {
    public Line (Vector3[] verts, Color color) : base (verts, color) {

    }

    public void Render () {

        GL.Begin (GL.LINES);

        // GL.Vertex(verts[0]);

        // GL.Vertex(verts[1]);

        RuntimeDebugDraw.Draw.DrawLine (verts[0], verts[1], Color.blue);
        GL.End ();

    }
}