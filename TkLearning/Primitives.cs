using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL4;

namespace TkLearning.Primitives
{
    public abstract class Primitive
    {
        public List<Vector3> Verts = new List<Vector3>();
        public float[] verts = { };
        public int DrawCount = 0;

        int VertexBufferObject;
        int VertexArrayObject;
        Shader ShaderProgram;

        public Primitive() { }

        public void Load()
        {
            VertexBufferObject = GL.GenBuffer();

            GL.BindBuffer(BufferTarget.ArrayBuffer, VertexBufferObject);
            Console.WriteLine(verts.Length);
            GL.BufferData(BufferTarget.ArrayBuffer, verts.Length * sizeof(float), verts, BufferUsageHint.StaticDraw);


            VertexArrayObject = GL.GenVertexArray();
            GL.BindVertexArray(VertexArrayObject);

            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);

            string vertShader = @"
#version 330 core
layout (location = 0) in vec3 aPosition;

void main()
{
    gl_Position = vec4(aPosition, 1.0);
}
";
            string fragShader = @"
#version 330 core
out vec4 FragColor;

void main()
{
    FragColor = vec4(1.0f, 0.5f, 0.2f, 1.0f);
}
";

            ShaderProgram = new Shader(vertShader, fragShader);

            ShaderProgram.Use();
        }

        public void Draw()
        {
            ShaderProgram.Use();
            GL.BindVertexArray(VertexArrayObject);
            GL.DrawArrays(PrimitiveType.Triangles, 0, DrawCount);
        }
    }

    public class Triangle : Primitive
    {
        public Triangle() : base()
        {
            DrawCount = 3;

            verts = new float[]{
                -0.5f, -0.5f, 0.0f, // BL
                0.5f, -0.5f, 0.0f, // BR
                0.0f, 0.5f, 0.0f, // T
            };
        }
    }

    public class Quad : Primitive
    {
        public Quad() : base()
        {
            DrawCount = 6;

            verts = new float[]
            {
                // Bottom Right Triangle
                -0.5f, -0.5f, 0.0f, // BL
                0.5f, -0.5f, 0.0f, // BR
                0.5f, 0.5f, 0.0f, // TR

                // Top left Triangle
                -0.5f, -0.5f, 0.0f, // BL
                0.5f, 0.5f, 0.0f, // TR
                -0.5f, 0.5f, 0.0f // TL
            };
        }
    }

    public class Pyramid : Primitive
    {

    }

    public class Cube : Primitive
    {

    }
}
