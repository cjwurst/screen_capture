using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LSystemDisplay : MonoBehaviour
{
    [SerializeField]
    Color[] colors;
    [SerializeField]
    bool isGradient = false;
    [SerializeField]
    int gradientCount;
    ColorPicker colorPicker;
    public int colorIndex
    {
        get { return colorPicker.i; }
        set { colorPicker.i = value; }
    }

    [Space(10)]
    [SerializeField]
    LSystem lSystem;
    string stringToDraw;

    [Space(10)]
    DrawTool drawTool;
    [SerializeField]
    Material lineMaterial;
    [SerializeField]
    float lineWidthStart;
    [SerializeField]
    float lineWidthEnd;

    [Space(10)]
    [SerializeField]
    bool iterateInstantly;
    [SerializeField]
    bool loop;

    void Start()
    {
        colorPicker = new ColorPicker(isGradient, gradientCount, colors);
        drawTool = new DrawTool(Vector3.zero, lineMaterial, colorPicker.color, lineWidthStart, lineWidthEnd);

        lSystem.Init(this);

        StartCoroutine(Init());
    }

    IEnumerator Init()
    {
        bool isFirstLoop = true;
        while (loop || isFirstLoop)
        {
            stringToDraw = lSystem.axiom;
            isFirstLoop = false;

            drawTool.Reset();
            colorPicker.Reset();

            drawTool.StartLine(Vector3.zero, lineMaterial, colorPicker.color);

            yield return StartCoroutine(lSystem.DrawString(stringToDraw, !iterateInstantly));
            yield return new WaitForSeconds(lSystem.iterationTime);

            for (int i = 0; i < lSystem.iterationCount; i++)
            {
                bool willRender = !iterateInstantly || i == lSystem.iterationCount - 1;

                drawTool.Reset();
                colorPicker.Reset();

                drawTool.StartLine(Vector3.zero, lineMaterial, colorPicker.color);
                stringToDraw = lSystem.IterateString(stringToDraw);

                yield return StartCoroutine(lSystem.DrawString(stringToDraw, willRender));
                if (willRender) yield return new WaitForSeconds(lSystem.iterationTime);
            }
        }

        /*bool isFirstLoop = true;
        while (loop || isFirstLoop)
        {
            stringToDraw = lSystem.axiom;
            isFirstLoop = false;
            if (iterateInstantly)
            {
                drawTool.Reset();
                colorPicker.Reset();

                for (int i = 0; i < lSystem.iterationCount; i++)
                    stringToDraw = lSystem.IterateString(stringToDraw);
                drawTool.StartLine(Vector3.zero, lineMaterial, colorPicker.color);
                yield return StartCoroutine(lSystem.DrawString(stringToDraw));

                yield return new WaitForSeconds(lSystem.iterationTime);
            }
            else
            {
                drawTool.Reset();
                colorPicker.Reset();

                drawTool.StartLine(Vector3.zero, lineMaterial, colorPicker.color);

                yield return StartCoroutine(lSystem.DrawString(stringToDraw));
                yield return new WaitForSeconds(lSystem.iterationTime);

                for (int i = 0; i < lSystem.iterationCount; i++)
                {
                    drawTool.Reset();
                    colorPicker.Reset();

                    drawTool.StartLine(Vector3.zero, lineMaterial, colorPicker.color);
                    stringToDraw = lSystem.IterateString(stringToDraw);

                    yield return StartCoroutine(lSystem.DrawString(stringToDraw));
                    yield return new WaitForSeconds(lSystem.iterationTime);
                }
            }
        }*/
    }

    public void ChangeColor()
    {
        colorPicker.Iterate();
        drawTool.StartLine(lineMaterial, colorPicker.color);
    }

    public void Jump(Vector2 target)
    {
        drawTool.StartLine(target, lineMaterial, colorPicker.color);
    }

    class ColorPicker
    {
        public Color color { get { return colors[i]; } }
        Color[] colors;
        public int i = 0;
        bool isGradient = false;

        public ColorPicker(bool _isGradient, int gradientCount, params Color[] _colors)
        {
            isGradient = _isGradient;
            if (!isGradient)
            {
                colors = _colors;
                return;
            }
        
            Color target = _colors[_colors.Length - 1];
            colors = new Color[gradientCount];
            float increment = 1f / (gradientCount - 1);
            for (int i = 0; i < gradientCount; i++)
                colors[i] = Color.Lerp(_colors[0], target, i * increment);
        }

        public void Iterate()
        {
            i++;
            if (i == colors.Length)
            {
                if (isGradient)
                {
                    i = colors.Length - 1;
                    return;
                }
                i = 0;
            }
        }

        public void Reset() { i = 0; }
    }

    public void DrawLine(Vector3 end) { drawTool.DrawLine(end); }

    class DrawTool
    {
        List<GameObject> lines = new List<GameObject>();

        float lineWidthStart;
        float lineWidthEnd;

        public DrawTool(Vector3 start, Material material, Color color, float _lineWidthStart, float _lineWidthEnd)
        {
            lineWidthStart = _lineWidthStart;
            lineWidthEnd = _lineWidthEnd;
            StartLine(start, material, color);
        }

        public void StartLine(Vector3 start, Material material, Color color)
        {
            GameObject line = new GameObject();
            LineRenderer lineRenderer = line.AddComponent<LineRenderer>();

            lineRenderer.material = material;
            lineRenderer.material.color = color;
            lineRenderer.startWidth = lineWidthStart;
            lineRenderer.endWidth = lineWidthEnd;
            lineRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            lineRenderer.receiveShadows = false;
            lineRenderer.useWorldSpace = false;
            lineRenderer.alignment = LineAlignment.TransformZ;
            lineRenderer.numCapVertices = 5;
            lineRenderer.numCornerVertices = 5;

            lineRenderer.positionCount = 1;
            lineRenderer.SetPosition(0, start);

            lines.Add(line);
        }
        public void StartLine(Material material, Color color)
        {
            LineRenderer lineRenderer = lines[lines.Count - 1].GetComponent<LineRenderer>();
            StartLine(lineRenderer.GetPosition(lineRenderer.positionCount - 1), material, color);
        }

        public void DrawLine(Vector3 end)
        {
            LineRenderer lineRenderer = lines[lines.Count - 1].GetComponent<LineRenderer>();
            lineRenderer.positionCount++;
            lineRenderer.SetPosition(lineRenderer.positionCount - 1, end);
        }

        public void Reset()
        {
            if (lines.Count == 0) return;
            foreach (GameObject line in lines) GameObject.Destroy(line);
            lines = new List<GameObject>();
        }
    }
}
