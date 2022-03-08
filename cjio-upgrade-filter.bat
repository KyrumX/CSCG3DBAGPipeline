venv\Scripts\activate && ^
cjio %1 ^
upgrade ^
lod_filter 2.2 ^
attribute_remove dak_type ^
attribute_remove data_area ^
attribute_remove data_coverage ^
attribute_remove documentnummer ^
attribute_remove geconstateerd ^
attribute_remove h_dak_50p ^
attribute_remove h_dak_70p ^
attribute_remove h_dak_max ^
attribute_remove h_dak_min ^
attribute_remove lod11_replace ^
attribute_remove kas_warenhuis ^
attribute_remove gid ^
attribute_remove ondergronds_type ^
attribute_remove oorspronkelijkbouwjaar ^
attribute_remove pw_actueel ^
attribute_remove pw_bron ^
attribute_remove reconstructie_methode ^
attribute_remove reconstruction_skipped ^
attribute_remove rmse_lod12 ^
attribute_remove rmse_lod13 ^
attribute_remove rmse_lod22 ^
attribute_remove rn ^
attribute_remove status ^
attribute_remove t_run ^
attribute_remove val3dity_codes_lod12 ^
attribute_remove val3dity_codes_lod13 ^
attribute_remove val3dity_codes_lod22 ^
attribute_remove voorkomenidentificatie ^
save %2 && ^
deactivate