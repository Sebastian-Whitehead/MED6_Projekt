# Load the readxl package
library(readxl)
library(dplyr)
library(ggplot2)
library(gridExtra)
library(grid)

# Specify the path to the Excel file
excel_file <- "../../test.xlsx"

# Read the Excel file into a data frame
my_data <- read_excel(excel_file)

version_colors = c("red", "blue")
version_names = c("Attack", "Mana")
agreeScala <- c("Strongly disagree", "Disagree", "Somewhat disagree", "Neutral", "Somewhat agree", "Agree", "Strongly agree")

# -----------------------------------------------------------------------

attack = my_data %>% filter(`Current played version` == 1)
mana = my_data %>% filter(`Current played version` == 2)

metaphorPreference <- ggplot(my_data, aes(y = factor(`Current played version`),
                                          x = `What do you think about this metaphor?`)) +
  geom_boxplot() +
  scale_y_discrete(labels = c("Attack", "Mana")) +
  ylab("")

# -----------------------------------------------------------------------

my_data$success_delta = abs(my_data$`How many motor imagery attempts do you think you performed? - Successfully` - my_data$`Success BCI`)
my_data$failed_delta = abs(my_data$`How many motor imagery attempts do you think you performed? - Failed` - my_data$`Failed BCI`)

max_delta = max(my_data$success_delta, my_data$failed_delta, na.rm = TRUE)
success_delta <- ggplot(my_data, aes(y = factor(`Current played version`),
                    x = success_delta)) +
  geom_boxplot() +
  scale_y_discrete(labels = c("Attack", "Mana")) +
  ylab("") +
  xlab("Succeded difference") +
  scale_x_continuous(breaks = seq(0, max_delta, by = 1))

failed_delta <- ggplot(my_data,
                       aes(
                         y = factor(`Current played version`),
                         x = failed_delta)) +
  geom_boxplot() +
  scale_y_discrete(labels = c("Attack", "Mana")) +
  ylab("") +
  xlab("Failed difference") +
  scale_x_continuous(breaks = seq(0, max_delta, by = 1))

# -----------------------------------------------------------------------

frustration_box <- ggplot(my_data, aes(y = factor(`Current played version`),
                                   x = `Performing motor imagery was frustrating.`)) +
  geom_boxplot() +
  scale_x_continuous(limits = c(1, 7), breaks = seq(1, 7, by = 1), labels = agreeScala) +
  scale_y_discrete(labels = c("Attack", "Mana")) +
  ylab("")

engagement_box <- ggplot(my_data, aes(y = factor(`Current played version`),
                                  x = `Performing motor imagery broke my engagement.`)) +
  geom_boxplot() +
  scale_x_continuous(limits = c(1, 7), breaks = seq(1, 7, by = 1), labels = agreeScala) +
  scale_y_discrete(labels = c("Attack", "Mana")) +
  ylab("")

concentration_box <- ggplot(my_data, aes(y = factor(`Current played version`),
                    x = `I could concentrate while performing motor imagery.`)) +
  geom_boxplot() +
  scale_x_continuous(limits = c(1, 7), breaks = seq(1, 7, by = 1), labels = agreeScala) +
  scale_y_discrete(labels = c("Attack", "Mana")) +
  ylab("")

frustration_hist <- ggplot(my_data,
                     aes(x = `Performing motor imagery was frustrating.`,
                         fill = factor(`Current played version`))) +
  geom_histogram(binwidth = .5, position = position_dodge(width = .75)) +
  scale_x_continuous(limits = c(1, 7), breaks = seq(1, 7, by = 1), labels = agreeScala) +
  scale_fill_manual(values = version_colors, labels = version_names) +
  labs(x = "Frustration", y = "Frequency", fill = "Version")

engagement_hist <- ggplot(my_data,
                        aes(x = `Performing motor imagery broke my engagement.`,
                            fill = factor(`Current played version`))) +
  geom_histogram(binwidth = .5, position = position_dodge(width = .75)) +
  scale_x_continuous(limits = c(1, 7), breaks = seq(1, 7, by = 1), labels = agreeScala) +
  scale_fill_manual(values = version_colors, labels = version_names) +
  labs(x = "Engagement", y = "Frequency", fill = "Version")

concentration_hist <- ggplot(my_data,
                        aes(x = `I could concentrate while performing motor imagery.`,
                            fill = factor(`Current played version`))) +
  geom_histogram(binwidth = .5, position = position_dodge(width = .75)) +
  scale_x_continuous(limits = c(1, 7), breaks = seq(1, 7, by = 1), labels = agreeScala) +
  scale_fill_manual(values = version_colors, labels = version_names) +
  labs(x = "Concentration", y = "Frequency", fill = "Version")

frustration_density <- ggplot(my_data, aes(x = `Performing motor imagery was frustrating.`,
                                     fill = factor(`Current played version`))) +
  geom_density(alpha = 0.5, aes(group = factor(`Current played version`))) +
  scale_x_continuous(limits = c(1, 7), breaks = seq(1, 7, by = 1), labels = agreeScala) +
  scale_fill_manual(values = version_colors, labels = version_names) +
  labs(x = "Concentration", y = "Density", fill = "Version")

engagement_density <- ggplot(my_data, aes(x = `Performing motor imagery broke my engagement.`,
                                     fill = factor(`Current played version`))) +
  geom_density(alpha = 0.5, aes(group = factor(`Current played version`))) +
  scale_x_continuous(limits = c(1, 7), breaks = seq(1, 7, by = 1), labels = agreeScala) +
  scale_fill_manual(values = version_colors, labels = version_names) +
  labs(x = "Concentration", y = "Density", fill = "Version")

concentration_density <- ggplot(my_data, aes(x = `I could concentrate while performing motor imagery.`,
                                     fill = factor(`Current played version`))) +
  geom_density(alpha = 0.5, aes(group = factor(`Current played version`))) +
  scale_x_continuous(limits = c(1, 7), breaks = seq(1, 7, by = 1), labels = agreeScala) +
  scale_fill_manual(values = version_colors, labels = version_names) +
  labs(x = "Concentration", y = "Density", fill = "Version")

# -----------------------------------------------------------------------

metaphorPreference
grid.arrange(success_delta, failed_delta, ncol = 1)
frus_eng_conc_hist <- grid.arrange(frustration_box, engagement_box, concentration_box, ncol = 1, top = textGrob("Performing motor imagery", gp=gpar(fontsize=20,font=3)))
frus_eng_conc_box <- grid.arrange(frustration_hist, engagement_hist, concentration_hist, ncol = 1, top = textGrob("Performing motor imagery", gp=gpar(fontsize=20,font=3)))
frus_eng_conc_density <- grid.arrange(frustration_density, engagement_density, concentration_density, ncol = 1, top = textGrob("Performing motor imagery", gp=gpar(fontsize=20,font=3)))
