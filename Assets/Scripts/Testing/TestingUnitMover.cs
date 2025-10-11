using Assets.HeroEditor4D.Common.Scripts.CharacterScripts;
using UnityEngine;

public class TestingUnitMover : MonoBehaviour
{
    private Character4D character4D;
    private AnimationManager animationManager;
    private Vector2 currentMoveDir;
    private void Start()
    {
        character4D = GetComponent<Character4D>();
        animationManager = GetComponent<AnimationManager>();
        currentMoveDir = Vector2Int.zero;
        character4D.SetDirection(GetRandom4D());
    }
    private void Update()
    {
        Vector2 moveDir = GetMoveDirNormalized();
        MoveUnit(moveDir);
        moveDir = CastTo4D(moveDir);
        if (currentMoveDir != moveDir)
        {
            currentMoveDir = moveDir;
            SetCharacterMoveDir(currentMoveDir);
        }
    }
    private void MoveUnit(Vector2 moveDir)
    {
        float moveSpeed = 3f;
        transform.position += (Vector3)(moveDir * (moveSpeed * Time.deltaTime));
    }
    private Vector2 GetMoveDirNormalized()
    {
        Vector2 moveDir = Vector2.zero;
        if (Input.GetKey(KeyCode.W))
        {
            moveDir.y += 1;
        }
        if (Input.GetKey(KeyCode.S))
        {
            moveDir.y -= 1;
        }
        if (Input.GetKey(KeyCode.D))
        {
            moveDir.x += 1;
        }
        if (Input.GetKey(KeyCode.A))
        {
            moveDir.x -= 1;
        }
        moveDir.Normalize();
        return moveDir;
    }
    private Vector2Int CastTo4D(Vector2 moveDir)
    {
        if (moveDir == Vector2.zero)
        {
            return Vector2Int.zero; // Return zero direction explicitly
        }

        return Mathf.Abs(moveDir.x) >= Mathf.Abs(moveDir.y)
            ? new Vector2Int((int)(moveDir.x / Mathf.Abs(moveDir.x)), 0) // 1 or -1
            : new Vector2Int(0, (int)(moveDir.y / Mathf.Abs(moveDir.y)));
    }
    private Vector2Int GetRandom4D()
    {
        return new[] { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right }[Random.Range(0, 4)];
    }
    private void SetCharacterMoveDir(Vector2 moveDir)
    {
        if (moveDir == Vector2Int.zero)
        {
            animationManager.SetState(Assets.HeroEditor4D.Common.Scripts.Enums.CharacterState.Idle);
        }
        else
        {
            animationManager.SetState(Assets.HeroEditor4D.Common.Scripts.Enums.CharacterState.Walk);
            character4D.SetDirection(moveDir);
        }
    }
}

