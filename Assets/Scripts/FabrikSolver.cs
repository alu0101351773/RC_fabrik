using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Código basado en el vídeo de Guis Caminiti
// https://youtu.be/_S2bNLpji2s
public class FabrikSolver : MonoBehaviour
{
    [SerializeField]
    public List<Transform> bones;
    float[] bonesLengths;

    [SerializeField]
    int solverIterations = 5;

    [SerializeField]
    Transform targetPosition;

    // Start is called before the first frame update
    void Start()
    {
        SetupBones();
    }


    void SetupBones()
    {
        foreach (GameObject bone in GameObject.FindGameObjectsWithTag("Bone"))
        {
            bones.Add(bone.GetComponent<Transform>());
        }

        bonesLengths = new float[bones.Count];

        for (int i = 0; i < bones.Count; i++)
        {
            if (i < bones.Count - 1)
            {
                bonesLengths[i] = (bones[i + 1].position - bones[i].position).magnitude;
            }
            else
            {
                bonesLengths[i] = 0f;
            }
        }
    }


    void Update()
    {
        SolveIK();
    }


    void SolveIK()
    {
        Vector3[] finalBonesPositions = new Vector3[bones.Count];

        // Establecemos la posición inicial de los huesos
        for (int i = 0; i < bones.Count; i++)
        {
            finalBonesPositions[i] = bones[i].position;
        }

        // Aplicamos FABRIK tantas veces como se indique
        for (int i = 0; i < solverIterations; i++)
        {
            finalBonesPositions = SolveForwardPositions(SolveBackwardPositions(finalBonesPositions));
        }

        // Aplicamos los resultados a cada hueso
        for (int i = 0; i < bones.Count; i++)
        {
            bones[i].position = finalBonesPositions[i];

            if (i != bones.Count - 1)
            {
                bones[i].rotation = Quaternion.LookRotation(finalBonesPositions[i + 1] - bones[i].position);
            }
            else
            {
                bones[i].rotation = Quaternion.LookRotation(targetPosition.position - bones[i].position);
            }
        }
    }


    Vector3[] SolveBackwardPositions(Vector3[] forwardPositions)
    {
        Vector3[] backwardPositions = new Vector3[forwardPositions.Length];

        for (int i = forwardPositions.Length - 1; i >= 0 ; i--)
        {
            if (i == forwardPositions.Length - 1)
            {
                backwardPositions[i] = targetPosition.position;
            }
            else
            {
                Vector3 posPrimaSiguiente = backwardPositions[i + 1];
                Vector3 posBaseActual = forwardPositions[i];
                Vector3 direccion = (posBaseActual - posPrimaSiguiente).normalized;
                float longitud = bonesLengths[i];
                backwardPositions[i] = posPrimaSiguiente + (direccion * longitud);
            }
        }
        return backwardPositions;
    }


    Vector3[] SolveForwardPositions(Vector3[] backwardPositions)
    {
        Vector3[] forwardPositions = new Vector3[backwardPositions.Length];

        for (int i = 0; i < backwardPositions.Length; i++)
        {
            if (i == 0)
            {
                forwardPositions[i] = bones[0].position;
            }
            else
            {
                Vector3 posPrimaActual = backwardPositions[i];
                Vector3 posPrimaSegundaAnterior = forwardPositions[i - 1];
                Vector3 direccion = (posPrimaActual - posPrimaSegundaAnterior).normalized;
                float longitud = bonesLengths[i - 1];
                forwardPositions[i] = posPrimaSegundaAnterior + (direccion * longitud);
            }
        }
        return forwardPositions;
    }
}
