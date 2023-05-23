# Load the readxl package
library(readxl)
library(dplyr)
library(ggplot2)
library(gridExtra)
library(grid)
library(tidyr)
library(reshape2)

setwd("C:/Users/Tonko/OneDrive/Dokumenter/School/MED6/Projekt")

# Specify the path to the Excel file
version_path <- "version.xlsx"
overall_path <- "overall.xlsx"

# Read the Excel file into a data frame
version <- read_excel(version_path)
overall <- read_excel(overall_path)

version_colors = c("red", "blue")
version_names = c("Interval", "Battery")
version_labels = c("Highly\nInterval", "", "Neutral", "", "Highly\nBattery")
agreeScala <- c("Strongly\ndisagree", "Disagree", "Somewhat\ndisagree", "Neutral", "Somewhat\nagree", "Agree", "Strongly\nagree")

# -----------------------------------------------------------------------

attack = version %>% filter(`Current played version` == 1)
mana = version %>% filter(`Current played version` == 2)

thisMetaphor <- ggplot(version, aes(
  y = factor(`Current played version`),
  x = `What do you think about this metaphor?`)) +
  geom_boxplot() +
  scale_y_discrete(labels = c("Attack", "Mana")) +
  ylab("")

maxPreference = 0.35
thisMetaphor_density <- ggplot(version, aes(
  x = `What do you think about this metaphor?`,
  fill = factor(`Current played version`))) +
  geom_density(alpha = 0.5, aes(group = factor(`Current played version`))) +
  scale_x_continuous(limits = c(1, 5), breaks = seq(1, 5, by = 1)) +
  scale_y_continuous(limits = c(0, maxPreference), breaks = seq(0, maxPreference, by = .05)) +
  scale_fill_manual(values = version_colors, labels = version_names) +
  labs(x = "Likeability", y = "Density", fill = "Version")

metaphorPreference_density <- ggplot(overall, aes(
  x = `Version likes`,
  fill = "red")) +
  geom_density(alpha = 0.5) +
  scale_x_continuous(limits = c(1, 5), breaks = seq(1, 5, by = 1), labels = version_labels) +
  scale_y_continuous(limits = c(0, maxPreference), breaks = seq(0, maxPreference, by = .05)) +
  scale_fill_manual(values = version_colors, labels = version_names, guide = FALSE) +
  labs(x = "Preferred metaphor", y = "Density", fill = "Version")

pacingPreference_density <- ggplot(overall, aes(
  x = `Pacing likes`,
  fill = "red")) +
  geom_density(alpha = 0.5) +
  scale_x_continuous(limits = c(1, 5), breaks = seq(1, 5, by = 1), labels = version_labels) +
  scale_y_continuous(limits = c(0, maxPreference), breaks = seq(0, maxPreference, by = .05)) +
  scale_fill_manual(values = version_colors, labels = version_names, guide = FALSE) +
  labs(x = "Preferred pacing", y = "Density", fill = "Version")

pacingPreference_density <- ggplot(overall, aes(
  x = `Pacing likes`,
  fill = "red")) +
  geom_density(alpha = 0.5) +
  scale_x_continuous(limits = c(1, 5), breaks = seq(1, 5, by = 1), labels = version_labels) +
  scale_y_continuous(limits = c(0, maxPreference), breaks = seq(0, maxPreference, by = .05)) +
  scale_fill_manual(values = version_colors, labels = version_names, guide = FALSE) +
  labs(x = "Preferred pacing", y = "Density", fill = "Version")

maxPreference = 5
thisMetaphor_hist <- ggplot(version, aes(
  x = `What do you think about this metaphor?`,
  fill = factor(`Current played version`))) +
  geom_histogram(alpha = 0.5, bins = 5, binwidth = 1, position = position_dodge(width = 0.5), colour = "black") +
  scale_x_continuous(limits = c(0, 6), breaks = seq(1, 5, by = 1)) +
  scale_y_continuous(limits = c(0, maxPreference), breaks = seq(0, maxPreference, by = 1)) +
  scale_fill_manual(values = version_colors, labels = version_names) +
  labs(x = "Likeability", y = "Density", fill = "Version")
thisMetaphor_hist

metaphorPreference_hist <- ggplot(overall, aes(x = `Version likes`, fill = "red")) +
  geom_histogram(alpha = 0.5, bins = 5, binwidth = 1, colour = "black") +
  scale_x_continuous(limits = c(0, 6), breaks = seq(1, 5, by = 1), labels = version_labels) +
  scale_y_continuous(limits = c(0, maxPreference), breaks = seq(0, maxPreference, by = 1)) +
  scale_fill_manual(values = version_colors, labels = version_names ) +
  labs(x = "Preferred metaphor", y = "Density")

pacingPreference_hist <- ggplot(overall, aes(x = `Pacing likes`, fill = "red")) +
  geom_histogram(alpha = 0.5, bins = 5, binwidth = 1, colour = "black") +
  scale_x_continuous(limits = c(0, 6), breaks = seq(1, 5, by = 1), labels = version_labels) +
  scale_y_continuous(limits = c(0, maxPreference), breaks = seq(0, maxPreference, by = 1)) +
  scale_fill_manual(values = version_colors, labels = version_names ) +
  labs(x = "Preferred pacing", y = "Density")

thisMetaphor_hist


# -----------------------------------------------------------------------

version$success_delta = abs(version$`How many motor imagery attempts do you think you performed? - Successfully` - version$`Success BCI`)
version$failed_delta = abs(version$`How many motor imagery attempts do you think you performed? - Failed` - version$`Failed BCI`)

max_delta = max(version$success_delta, version$failed_delta, na.rm = TRUE)
success_delta <- ggplot(version, aes(y = factor(`Current played version`),
                    x = success_delta)) +
  geom_boxplot() +
  scale_y_discrete(labels = version_names) +
  ylab("") +
  xlab("Succeded difference") +
  scale_x_continuous(breaks = seq(0, max_delta, by = 1))

failed_delta <- ggplot(version,
                       aes(
                         y = factor(`Current played version`),
                         x = failed_delta)) +
  geom_boxplot() +
  scale_y_discrete(labels = version_names) +
  ylab("") +
  xlab("Failed difference") +
  scale_x_continuous(breaks = seq(0, max_delta, by = 1))

# -----------------------------------------------------------------------

frustration_box <- ggplot(version, aes(y = factor(`Current played version`),
                                   x = `Performing motor imagery was frustrating.`)) +
  geom_boxplot() +
  scale_x_continuous(limits = c(1, 7), breaks = seq(1, 7, by = 1), labels = agreeScala) +
  scale_y_discrete(labels = c("Attack", "Mana")) +
  ylab("")

engagement_box <- ggplot(version, aes(y = factor(`Current played version`),
                                  x = `Performing motor imagery broke my engagement.`)) +
  geom_boxplot() +
  scale_x_continuous(limits = c(1, 7), breaks = seq(1, 7, by = 1), labels = agreeScala) +
  scale_y_discrete(labels = c("Attack", "Mana")) +
  ylab("")

concentration_box <- ggplot(version, aes(y = factor(`Current played version`),
                    x = `I could concentrate while performing motor imagery.`)) +
  geom_boxplot() +
  scale_x_continuous(limits = c(1, 7), breaks = seq(1, 7, by = 1), labels = agreeScala) +
  scale_y_discrete(labels = version_names) +
  ylab("")

frustration_hist <- ggplot(version,
                     aes(x = `Performing motor imagery was frustrating.`,
                         fill = factor(`Current played version`))) +
  geom_histogram(binwidth = .5, position = position_dodge(width = .75), alpha = 0.5) +
  scale_x_continuous(limits = c(1, 7), breaks = seq(1, 7, by = 1), labels = agreeScala) +
  scale_fill_manual(values = version_colors, labels = version_names ) +
  labs(x = "Frustration", y = "Frequency", fill = "Version")
frustration_hist

engagement_hist <- ggplot(version,
                        aes(x = `Performing motor imagery broke my engagement.`,
                            fill = factor(`Current played version`))) +
  geom_histogram(binwidth = .5, position = position_dodge(width = .75), alpha = 0.5) +
  scale_x_continuous(limits = c(1, 7), breaks = seq(1, 7, by = 1), labels = agreeScala) +
  scale_fill_manual(values = version_colors, labels = version_names) +
  labs(x = "Engagement", y = "Frequency", fill = "Version") + theme(
    legend.position = c(.99, .5),
    legend.justification = c("right"),
    legend.box.just = "right",
    legend.margin = margin(6, 6, 6, 6)
  )
engagement_hist

concentration_hist <- ggplot(version,
                        aes(x = `I could concentrate while performing motor imagery.`,
                            fill = factor(`Current played version`))) +
  geom_histogram(binwidth = .5, position = position_dodge(width = .75), alpha = 0.5) +
  scale_x_continuous(limits = c(1, 7), breaks = seq(1, 7, by = 1), labels = agreeScala) +
  scale_fill_manual(values = version_colors, labels = version_names ) +
  labs(x = "Concentration", y = "Frequency", fill = "Version")

frustration_density <- ggplot(version, aes(x = `Performing motor imagery was frustrating.`,
                                     fill = factor(`Current played version`))) +
  geom_density(alpha = 0.5, aes(group = factor(`Current played version`))) +
  scale_x_continuous(limits = c(1, 7), breaks = seq(1, 7, by = 1), labels = agreeScala) +
  scale_fill_manual(values = version_colors, labels = version_names ) +
  labs(x = "Frustration", y = "Density", fill = "Version")
frustration_density

engagement_density <- ggplot(version, aes(x = `Performing motor imagery broke my engagement.`,
                                     fill = factor(`Current played version`))) +
  geom_density(alpha = 0.5, aes(group = factor(`Current played version`))) +
  scale_x_continuous(limits = c(1, 7), breaks = seq(1, 7, by = 1), labels = agreeScala) +
  scale_fill_manual(values = version_colors, labels = version_names) +
  labs(x = "Engagement", y = "Density", fill = "Version") + theme(
    legend.position = c(.99, .5),
    legend.justification = c("right"),
    legend.box.just = "right",
    legend.margin = margin(6, 6, 6, 6)
  )
engagement_density

concentration_density <- ggplot(version, aes(x = `I could concentrate while performing motor imagery.`,
                                             fill = factor(`Current played version`))) +
  geom_density(alpha = 0.5, aes(group = factor(`Current played version`))) +
  scale_x_continuous(limits = c(1, 7), breaks = seq(1, 7, by = 1), labels = agreeScala) +
  scale_fill_manual(values = version_colors, labels = version_names) +
  labs(x = "Concentration", y = "Density", fill = "Version")
concentration_density

timing_density <- ggplot(version, aes(x = `The timing of the motor imagery made sense.`,
                                      fill = factor(`Current played version`))) +
  geom_density(alpha = 0.5, aes(group = factor(`Current played version`))) +
  scale_x_continuous(limits = c(1, 7), breaks = seq(1, 7, by = 1), labels = agreeScala) +
  scale_fill_manual(values = version_colors, labels = version_names) +
  labs(x = "Timing", y = "Density", fill = "Version")
timing_density

natural_density <- ggplot(version, aes(
  x = `It felt natural to perform motor imagery in this context.`,
  fill = factor(`Current played version`))) +
  geom_density(alpha = 0.5, aes(group = factor(`Current played version`))) +
  scale_x_continuous(limits = c(1, 7), breaks = seq(1, 7, by = 1), labels = agreeScala) +
  scale_fill_manual(values = version_colors, labels = version_names ) +
  labs(x = "Natural", y = "Density", fill = "Version") + theme(
    legend.position = c(.99, .5),
    legend.justification = c("right"),
    legend.box.just = "right",
    legend.margin = margin(6, 6, 6, 6)
  )
natural_density

trigger_density <- ggplot(version, aes(x = `I found the triggers for motor imagery logical.`,
                                       fill = factor(`Current played version`))) +
  geom_density(alpha = 0.5, aes(group = factor(`Current played version`))) +
  scale_x_continuous(limits = c(1, 7), breaks = seq(1, 7, by = 1), labels = agreeScala) +
  scale_fill_manual(values = version_colors, labels = version_names ) +
  labs(x = "Trigger", y = "Density", fill = "Version")
trigger_density

# -----------------------------------------------------------------------

# -----------------------------------------------------------------------

general_questions <- overall[,-1:-3]
general_questions$`I found the game confusing to play` = abs(8 - general_questions$`I found the game confusing to play`)
general_questions$`I felt bored while playing the game` = abs(8 - general_questions$`I felt bored while playing the game`)
reversed = abs(8 - general_questions)

general_questions$mean = rowMeans(reversed)

# Plot multiple values with different colors
general_collective <- ggplot(general_questions, aes(x = mean, fill = "red")) +
  geom_density(alpha = 0.5) +
  scale_x_continuous(limits = c(1, 7), breaks = seq(1, 7, by = 1), labels = agreeScala) +
  scale_fill_manual(values = version_colors ) +
  labs(x = "", y = "Density")
general_collective

m_df <- as.data.frame(reversed) # convert matrix to data frame
m_long <- m_df %>% gather(variable, value) # reshape data to long format
general_boxplots <- ggplot(m_long, aes(y = variable, x = value)) +
  geom_boxplot() +
  scale_x_continuous(limits = c(1, 7), breaks = seq(1, 7, by = 1), labels = agreeScala) +
  labs(x = "", y = "")
general_boxplots


# -----------------------------------------------------------------------

# -----------------------------------------------------------------------

#thisMetaphor
#thisMetaphor_density
#metaphorPreference_density
#pacingPreference_density
grid.arrange(pacingPreference_density, metaphorPreference_density, thisMetaphor_density, nrow = 1)
#grid.arrange(pacingPreference_hist, metaphorPreference_hist, thisMetaphor_hist, nrow = 1)
#grid.arrange(pacingPreference_hist + geom_density(alpha = 0.5), metaphorPreference_hist + geom_density(alpha = 0.5), thisMetaphor_hist + geom_density(alpha = 0.5), nrow = 1)
grid.arrange(success_delta, failed_delta, ncol = 1)
#frus_eng_conc_hist <- grid.arrange(frustration_box, engagement_box, concentration_box, ncol = 1, top = textGrob("Performing motor imagery", gp=gpar(fontsize=20,font=3)))
#frus_eng_conc_box <- grid.arrange(frustration_hist, engagement_hist, concentration_hist, ncol = 1, top = textGrob("Performing motor imagery", gp=gpar(fontsize=20,font=3)))
#frus_eng_conc_box_dens <- grid.arrange(frustration_hist + geom_density(alpha = 0.5), engagement_hist + geom_density(alpha = 0.5), concentration_hist + geom_density(alpha = 0.5), ncol = 1, top = textGrob("Performing motor imagery", gp=gpar(fontsize=20,font=3)))
#frus_eng_conc_density <- grid.arrange(frustration_density, engagement_density, concentration_density, ncol = 1, top = textGrob("Performing motor imagery", gp=gpar(fontsize=20,font=3)))
#frus_eng_conc_density <- grid.arrange(timing_density, natural_density, trigger_density, ncol = 1, top = textGrob("P", gp=gpar(fontsize=20,font=3)))
frus_eng_conc_density <- grid.arrange(frustration_density, engagement_density, concentration_density, timing_density, natural_density, trigger_density, ncol = 2, top = textGrob("Performing motor imagery", gp=gpar(fontsize=20,font=3)))
general_collective
general_boxplots
