#pragma once

#include <malloc.h>
#include <stdbool.h>
#include <stdint.h>
#include <stdio.h>
#include <stdlib.h>
#include <string.h>

#include <freertos/FreeRTOS.h>
#include <freertos/queue.h>
#include <freertos/semphr.h>

#include "driver/sdmmc_host.h"

#define dprintf printf

#if defined(__cplusplus)
extern "C"
{
#endif

    void app_main();

#if defined(__cplusplus)
}
#endif