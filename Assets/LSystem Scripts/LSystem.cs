using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "L-System")]
public class LSystem : ScriptableObject
{
    [SerializeField]
    LRule[] rules;

    [Space(10)]
    [SerializeField]
    DrawInstruction[] drawInstructions;

    [Space(10)]
    public int iterationCount;
    public float iterationTime;
    public float drawTime;

    [Space(10)]
    public float turnAngle;
    float runtimeTurnAngle;
    float turnRadians { get { return 2f * Mathf.PI * runtimeTurnAngle / 360f; } }
    public float drawDistance;

    [Space(10)]
    public string axiom;

    protected Vector2 turtlePosition = Vector2.zero;
    protected float turtleAngle = 0f;

    protected LSystemDisplay display;

    protected List<Action> eraseCommands = new List<Action>();

    float savedAngle;

    public void Init(LSystemDisplay _display)
    {
        display = _display;

        runtimeTurnAngle = turnAngle;
    }

    public virtual string IterateString(string s)
    {
        char[] chars = s.ToCharArray();
        string output = "";
        for (int i = 0; i < chars.Length; i++)
        {
            bool ruleApplied = false;
            foreach (LRule rule in rules)
            {
                if (chars[i] == rule.input)
                {
                    output += rule.output;
                    ruleApplied = true;
                }
            }
            if (!ruleApplied)
                output += chars[i];
        }
        MonoBehaviour.print(output + "\nCharacter Count: " + output.Length + " Sign Count: " + output.Count((x) => x == '+' || x == '-'));
        return output;
    }

    void Reset()
    {
        turtlePosition = Vector2.zero;
        turtleAngle = 0f;
    }
    
    public virtual IEnumerator DrawString(string s, bool willRender)
    {
        Reset();

        List<Tuple<float, Vector2>> savedStates = new List<Tuple<float, Vector2>>();
        List<int> savedColors = new List<int>();

        char[] drawChars = s.ToCharArray();
        for (int i = 0; i < drawChars.Length; i++)
        {
            foreach(DrawInstruction drawInstruction in drawInstructions)
            {
                if (drawInstruction.input == drawChars[i])
                {
                    switch (drawInstruction.output)
                    {
                        case DrawInstruction.Instruction.Forward:
                            if (!willRender) break;
                            DrawForward();
                            if (drawTime > 0f) yield return new WaitForSeconds(drawTime);
                            break;

                        case DrawInstruction.Instruction.TurnLeft:
                            if (!willRender) break;
                            turtleAngle += turnRadians;
                            break;

                        case DrawInstruction.Instruction.TurnRight:
                            if (!willRender) break;
                            turtleAngle -= turnRadians;
                            break;

                        case DrawInstruction.Instruction.HalfAngle:
                            runtimeTurnAngle /= 2f;
                            break;

                        case DrawInstruction.Instruction.Save:
                            if (!willRender) break;
                            savedStates.Add(new Tuple<float, Vector2>(turtleAngle, turtlePosition));
                            savedColors.Add(display.colorIndex);
                            break;

                        case DrawInstruction.Instruction.Load:
                            if (!willRender) break;
                            Tuple<float, Vector2> state = savedStates[savedStates.Count - 1];
                            savedStates.RemoveAt(savedStates.Count - 1);
                            turtleAngle = state.Item1;
                            turtlePosition = state.Item2;

                            display.colorIndex = savedColors[savedColors.Count - 1];
                            savedColors.RemoveAt(savedColors.Count - 1);

                            display.Jump(turtlePosition);
                            break;

                        case DrawInstruction.Instruction.ChangeColor:
                            if (!willRender) break;
                            display.ChangeColor();
                            break;
                    }
                }
            }
        }
    }

    protected void DrawForward()
    {
        Vector2 target = turtlePosition + new Vector2(Mathf.Cos(turtleAngle) * drawDistance, Mathf.Sin(turtleAngle) * drawDistance);
        display.DrawLine(target);
        turtlePosition = target;
    }

    [Serializable]
    public struct LRule
    {
        public char input;
        public WeightedOutput[] outputs; 
        public string output
        {
            get
            {
                float sum = 0;
                foreach (WeightedOutput o in outputs) sum += o.weight;
                float roll = sum * UnityEngine.Random.value;
                float partialSum = 0;
                foreach (WeightedOutput o in outputs)
                {
                    partialSum += o.weight;
                    if (partialSum >= roll) return o.output;
                }
                return " Something has gone terribly wrong. ";
            }
        }

        [Serializable]
        public struct WeightedOutput
        {
            public float weight;
            public string output;
        }
    }

    [Serializable]
    public struct DrawInstruction
    {
        public enum Instruction
        {
            Forward,
            TurnLeft,
            TurnRight,
            HalfAngle,
            Save,
            Load,
            ChangeColor
        }
        public char input;
        public WeightedOutput[] outputs;

        public Instruction output
        {
            get
            {
                float sum = 0;
                foreach (WeightedOutput o in outputs) sum += o.weight;
                float roll = sum * UnityEngine.Random.value;
                float partialSum = 0;
                foreach (WeightedOutput o in outputs)
                {
                    partialSum += o.weight;
                    if (partialSum >= roll) return o.output;
                }
                return Instruction.Forward;
            }
        }

        [Serializable]
        public struct WeightedOutput
        {
            public float weight;
            public Instruction output;
        }
    }
}
