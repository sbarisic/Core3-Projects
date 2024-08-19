#include <core3.h>
#include <esp_intr_alloc.h>
#include <esp_timer.h>

// Uncomment for debug
#if 1
#if defined(dprintf)
#undef dprintf
#define dprintf(...)
#endif
#endif

#define GATE_GPIO_OPEN_OUT GPIO_NUM_21
#define GATE_GPIO_CLOSE_OUT GPIO_NUM_22

#define GATE_GPIO_OPEN_IN GPIO_NUM_16
#define GATE_GPIO_CLOSE_IN GPIO_NUM_17

#define GATE_GPIO_MAIN_RELAY GPIO_NUM_23

static QueueHandle_t gpio_evt_queue = NULL;
volatile static int main_relay_sleep_period = 0;
volatile static int gate_open = 0;
volatile static int gate_close = 0;

static void IRAM_ATTR gpio_isr_handler(void *arg)
{
    uint32_t gpio_num = (uint32_t)arg;
    xQueueSendFromISR(gpio_evt_queue, &gpio_num, NULL);
}

void enable_220v()
{
    dprintf("Turning on 220V!\n");
    gpio_set_level(GATE_GPIO_MAIN_RELAY, 0);
}

void disable_220v()
{
    dprintf("Turning off 220V!\n");
    gpio_set_level(GATE_GPIO_MAIN_RELAY, 1);
}

static void IRAM_ATTR timer_countdown_220v(void *args)
{
    if ((main_relay_sleep_period > 0) && (gate_open == 0 && gate_close == 0))
    {
        main_relay_sleep_period = main_relay_sleep_period - 1;

        if (main_relay_sleep_period == 0)
        {
            uint32_t gpio_num = (uint32_t)GATE_GPIO_MAIN_RELAY;
            xQueueSendFromISR(gpio_evt_queue, &gpio_num, NULL);
            // xQueueSend(gpio_evt_queue, (void *)GATE_GPIO_MAIN_RELAY, portMAX_DELAY);
        }
    }
}

void configure_gpio_interrupt(gpio_num_t gpionum)
{
    gpio_set_direction(gpionum, GPIO_MODE_INPUT);
    gpio_pullup_dis(gpionum);
    gpio_pulldown_en(gpionum);

    gpio_set_intr_type(gpionum, GPIO_INTR_ANYEDGE);
    gpio_isr_handler_add(gpionum, gpio_isr_handler, (void *)gpionum);
}

void configure_gpio_set_output(gpio_num_t gpionum, int out)
{
    gpio_set_direction(gpionum, GPIO_MODE_OUTPUT);

    if (out == 1)
    {
        gpio_pulldown_dis(gpionum);
        gpio_pullup_en(gpionum);
    }
    else
    {
        gpio_pullup_dis(gpionum);
        gpio_pulldown_en(gpionum);
    }

    gpio_set_level(gpionum, out);
}

void app_setup()
{
}

void app_main()
{
    uint32_t io_num = 0;

    dprintf("Starting app!\n");
    configure_gpio_set_output(GATE_GPIO_MAIN_RELAY, 1);
    configure_gpio_set_output(GATE_GPIO_OPEN_OUT, 0);
    configure_gpio_set_output(GATE_GPIO_CLOSE_OUT, 0);

    gpio_evt_queue = xQueueCreate(16, sizeof(uint32_t));

    gpio_install_isr_service(0);
    configure_gpio_interrupt(GATE_GPIO_CLOSE_IN);
    configure_gpio_interrupt(GATE_GPIO_OPEN_IN);

    dprintf("Starting timer\n");
    esp_timer_create_args_t timer_can_send_args = {.callback = timer_countdown_220v,
                                                   .arg = NULL,
                                                   .dispatch_method = ESP_TIMER_TASK,
                                                   .name = "timer_countdown_220v",
                                                   .skip_unhandled_events = false};

    esp_timer_handle_t task_can_send_timer;
    esp_timer_create(&timer_can_send_args, &task_can_send_timer);
    esp_timer_start_periodic(task_can_send_timer, 100000); // 100 ms

    dprintf("Done!\n");
    while (true)
    {
        if (xQueueReceive(gpio_evt_queue, &io_num, portMAX_DELAY))
        {
            if (io_num == GATE_GPIO_MAIN_RELAY)
            {
                disable_220v();
            }
            else if ((io_num == GATE_GPIO_OPEN_IN) || (io_num == GATE_GPIO_CLOSE_IN))
            {
                main_relay_sleep_period = 8;
                gate_open = gpio_get_level(GATE_GPIO_OPEN_IN);
                gate_close = gpio_get_level(GATE_GPIO_CLOSE_IN);

                if (gate_open != 0 || gate_close != 0)
                    enable_220v();
            }

            if (gate_open)
            {
                dprintf("Opening gate!\n");
                gpio_set_level(GATE_GPIO_CLOSE_OUT, 0);
                gpio_set_level(GATE_GPIO_OPEN_OUT, 1);
            }
            else if (gate_close)
            {
                dprintf("Closing gate!\n");
                gpio_set_level(GATE_GPIO_OPEN_OUT, 0);
                gpio_set_level(GATE_GPIO_CLOSE_OUT, 1);
            }
            else
            {
                dprintf("Resetting...\n");
                gpio_set_level(GATE_GPIO_CLOSE_OUT, 0);
                gpio_set_level(GATE_GPIO_OPEN_OUT, 0);
            }
        }
    }
}