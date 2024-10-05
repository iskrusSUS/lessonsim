using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController : MonoBehaviour
{
    public WaypointManager waypointManager;  // Ссылка на менеджер точек
    public float stoppingDistance = 1f;      // Дистанция, при которой мы считаем точку достигнутой
    public float speed = 10f;                // Скорость движения вперед
    public float turnSpeed = 5f;             // Скорость поворота
    public int currentWaypointIndex = 0;     // Индекс текущей точки

    public float stuckTimeThreshold = 2f;    // Время, через которое мы считаем, что застряли
    public float reverseTime = 1f;           // Время заднего хода, чтобы выйти из препятствия
    private bool reversing = false;          // Флаг реверса
    private float reverseTimer = 0f;         // Таймер для реверса
    private float stuckTimer = 0f;           // Таймер для отслеживания застревания
    private Vector3 lastPosition;            // Для проверки движения

    private SCC_InputProcessor inputProcessor;  // Ссылка на скрипт обработки вводов

    void Start()
    {
        inputProcessor = GetComponent<SCC_InputProcessor>();

        if (waypointManager == null)
        {
            Debug.LogError("WaypointManager не присвоен AIController!");
        }

        lastPosition = transform.position;  // Сохраняем начальную позицию
    }

    void Update()
    {
        // Проверяем, застряли ли мы
        if (!reversing && IsStuck())
        {
            // Начинаем реверс
            reversing = true;
            reverseTimer = reverseTime;
        }

        if (reversing)
        {
            ReverseMovement();
        }
        else
        {
            MoveTowardsWaypoint();
        }

        // Обновляем последнюю позицию
        lastPosition = transform.position;
    }

    // Логика движения к точкам
    void MoveTowardsWaypoint()
    {
        if (currentWaypointIndex >= waypointManager.GetWaypointCount())
        {
            // Все точки пройдены, остановить движение
            inputProcessor.inputs.throttleInput = 0f;
            inputProcessor.inputs.steerInput = 0f;
            return;
        }

        Transform targetWaypoint = waypointManager.GetWaypoint(currentWaypointIndex);
        Vector3 direction = (targetWaypoint.position - transform.position).normalized;
        float distance = Vector3.Distance(transform.position, targetWaypoint.position);

        // Определение угла поворота к цели
        Vector3 targetDirection = targetWaypoint.position - transform.position;
        float angleToTarget = Vector3.SignedAngle(transform.forward, targetDirection, Vector3.up);

        // Управление поворотом (используем turnSpeed)
        float steerInput = Mathf.Clamp(angleToTarget / 45f, -1f, 1f);  // Нормализуем угол в диапазон от -1 до 1
        inputProcessor.inputs.steerInput = Mathf.MoveTowards(inputProcessor.inputs.steerInput, steerInput, turnSpeed * Time.deltaTime);

        // Управление газом (используем speed)
        if (distance > stoppingDistance)
        {
            inputProcessor.inputs.throttleInput = Mathf.MoveTowards(inputProcessor.inputs.throttleInput, 1f, speed * Time.deltaTime);
        }
        else
        {
            // Точка достигнута, переключаемся на следующую
            inputProcessor.inputs.throttleInput = 0f;
            currentWaypointIndex++;
        }
    }

    // Логика заднего хода
    void ReverseMovement()
    {
        inputProcessor.inputs.throttleInput = -1f;  // Включаем задний ход
        inputProcessor.inputs.steerInput = 0f;      // Не поворачиваем

        reverseTimer -= Time.deltaTime;
        if (reverseTimer <= 0f)
        {
            reversing = false;  // Останавливаем реверс
        }
    }

    // Проверка застревания
    bool IsStuck()
    {
        // Если объект почти не изменил свою позицию за определенный период времени, считаем его застрявшим
        if (Vector3.Distance(transform.position, lastPosition) < 0.1f)
        {
            stuckTimer += Time.deltaTime;
            if (stuckTimer > stuckTimeThreshold)
            {
                return true;
            }
        }
        else
        {
            // Если мы двигаемся, сбрасываем таймер застревания
            stuckTimer = 0f;
        }

        return false;
    }
}
